using LojistikAPI.Data;
using LojistikAPI.Entities;
using LojistikAPI.Hubs;
using LojistikAPI.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LojistikAPI.Services
{
    public class OfferService : IOfferService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly EmailService _emailService;

        // Bağımlılıkları (Dependency Injection) artık Controller değil, bu Servis alıyor
        public OfferService(AppDbContext context, IHubContext<NotificationHub> notificationHub, EmailService emailService)
        {
            _context = context;
            _notificationHub = notificationHub;
            _emailService = emailService;
        }

        public async Task<int> CreateOfferAsync(CreateOfferDto dto)
        {
            // 1. İlan gerçekten var mı kontrol edelim
            var shipmentExists = await _context.Shipments.AnyAsync(s => s.Id == dto.ShipmentId);
            if (!shipmentExists)
            {
                throw new Exception($"DİKKAT: Veritabanında {dto.ShipmentId} numaralı bir ilan yok! Lütfen önce Shipments kısmından yeni bir ilan oluştur.");
            }

            // 2. Veritabanında hiç firma var mı diye kontrol ediyoruz (Yoksa Test Firması Oluştur)
            var company = await _context.Companies.FirstOrDefaultAsync();
            if (company == null)
            {
                string randomEmail = "test" + Guid.NewGuid().ToString().Substring(0, 5) + "@lojistik.com";

                var dummyUser = new User
                {
                    FullName = "Test Lojistik",
                    Email = randomEmail,
                    Password = BCrypt.Net.BCrypt.HashPassword("123"), // Şifreyi güvenli hale getirdik
                    Role = "Firma",
                    PhoneNumber = "555"
                };
                _context.Users.Add(dummyUser);
                await _context.SaveChangesAsync();

                company = new Company
                {
                    Name = "Test Nakliyat A.Ş.",
                    TaxNo = "111222333",
                    LicenseNo = "K1-999",
                    Rating = 5.0,
                    UserId = dummyUser.Id
                };
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
            }

            // 3. Teklifi Kaydet
            var offer = new Offer
            {
                Price = dto.Price,
                ValidUntil = dto.ValidUntil,
                Status = "Beklemede",
                ShipmentId = dto.ShipmentId,
                CompanyId = company.Id
            };

            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            // 4. Bildirimleri Gönder
            var shipment = await _context.Shipments
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.Id == dto.ShipmentId);

            if (shipment != null)
            {
                await _notificationHub.Clients.Group($"User_{shipment.CustomerId}")
                    .SendAsync("ReceiveNotification", $"İlanınıza {dto.Price} TL tutarında yeni bir teklif geldi!");

                if (shipment.Customer != null)
                {
                    await _emailService.SendEmailAsync(
                        toEmail: shipment.Customer.Email,
                        subject: "Yeni Nakliye Teklifi!",
                        message: $"Sayın {shipment.Customer.FullName}, yükünüz için {dto.Price} TL teklif verilmiştir."
                    );
                }
            }

            // İşlem başarılıysa eklenen teklifin ID'sini döndür
            return offer.Id;
        }
    }
}