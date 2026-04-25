using LojistikAPI.Data;
using LojistikAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojistikAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShipmentsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. TÜM YÜK İLANLARINI LİSTELE
        [HttpGet]
        public async Task<IActionResult> GetShipments()
        {
            var shipments = await _context.Shipments.ToListAsync();
            return Ok(shipments);
        }

        // 2. YENİ YÜK İLANI EKLE
        [HttpPost]
        public async Task<IActionResult> CreateShipment(Shipment newShipment)
        {
            _context.Shipments.Add(newShipment);
            await _context.SaveChangesAsync();
            return Ok("Yük ilanı başarıyla sisteme kaydedildi!");
        }
    }
}