using LojistikAPI.Data;
using LojistikAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace LojistikAPI.Services
{
    // BackgroundService, web sitesi ayakta olduğu sürece arka planda durmadan çalışan bir işçidir.
    public class FuelPriceUpdateService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public FuelPriceUpdateService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // ✅ EKLENEN KOD: Uygulama ilk açıldığında SQL Server'ın uyanması ve bağlantı kurması için 5 saniye bekle. 
            // Bu sayede "Timeout (Zaman Aşımı)" hatasından kurtulmuş oluyoruz!
            await Task.Delay(5000, stoppingToken);

            // Web sitesi kapanana kadar bu döngü dönecek
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // 1. GERÇEK SENARYO: Burada EPDK'nın web sitesine bağlanıp (Web Scraping) güncel veri çekilir.
                    // Biz test etmek ve sistemin nasıl akıllandığını görmek için piyasa dalgalanmasını simüle ediyoruz.

                    Random rnd = new Random();
                    // Fiyatı 41.00 TL ile 44.00 TL arasında dalgalandıralım
                    double simulatedPrice = 41.00 + (rnd.NextDouble() * 3);

                    var newFuelPrice = new FuelPrice
                    {
                        PricePerLiter = Math.Round(simulatedPrice, 2),
                        FuelType = "Motorin"
                    };

                    context.FuelPrices.Add(newFuelPrice);
                    await context.SaveChangesAsync();

                    // Geliştirici olarak terminalde görebilmemiz için log bırakıyoruz
                    Console.WriteLine($"[OTOMATİK SİSTEM] Güncel Mazot Fiyatı Veritabanına İşlendi: {newFuelPrice.PricePerLiter} TL");
                }

                // 2. ÇALIŞMA SIKLIĞI
                // Gerçek hayatta bu işçi günde 1 kez çalışır: await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                // Ancak bizim hemen test edebilmemiz için şimdilik "1 Dakikada Bir" çalışacak şekilde ayarlıyoruz.
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}