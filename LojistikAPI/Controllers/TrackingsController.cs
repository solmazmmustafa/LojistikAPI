using LojistikAPI.Data;
using LojistikAPI.Entities;
using LojistikAPI.Hubs;
using LojistikAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LojistikAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<TrackingHub> _hubContext; // Telsiz Merkezini içeri alıyoruz

        public TrackingsController(AppDbContext context, IHubContext<TrackingHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("update-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto dto)
        {
            // 1. Şoförden gelen konumu veritabanına geçmiş (history) olarak kaydet
            var tracking = new Tracking
            {
                ShipmentId = dto.ShipmentId,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Timestamp = DateTime.UtcNow
            };

            _context.Trackings.Add(tracking);
            await _context.SaveChangesAsync();

            // 2. İŞTE SİHİRLİ DOKUNUŞ!
            // O yükü takip eden müşterilerin ekranına, sayfayı yenilemelerine gerek kalmadan anında yeni konumu fırlatıyoruz!
            await _hubContext.Clients.Group($"Shipment_{dto.ShipmentId}")
                .SendAsync("ReceiveLocationUpdate", new
                {
                    lat = dto.Latitude,
                    lng = dto.Longitude
                });

            return Ok(new { message = "Konum veritabanına işlendi ve müşteriye canlı iletildi!" });
        }
    }
}