using LojistikAPI.Data;
using LojistikAPI.Entities;
using LojistikAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LojistikAPI.Controllers
{
    // [Authorize]  <--- GÜVENLİĞİ GEÇİCİ OLARAK SUSTURDUK (Başına // koyduk)
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShipmentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetShipments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            pageSize = Math.Clamp(pageSize, 1, 50);

            var totalCount = await _context.Shipments.CountAsync();

            var shipments = await _context.Shipments
                .AsNoTracking()
                .OrderByDescending(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new ShipmentResponseDto
                {
                    Id = s.Id,
                    OriginAddress = s.OriginAddress,
                    DestinationAddress = s.DestinationAddress,
                    Weight = s.Weight,
                    Status = s.Status
                })
                .ToListAsync();

            return Ok(new
            {
                data = shipments,
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Güvenliği kapattığımız için sisteme giriş yapan bir kullanıcı yok. 
            // Bu yüzden "userId" bilgisini geçici olarak "1" (Admin veya Test kullanıcısı) kabul ediyoruz.
            int userId = 1;

            var shipment = new Shipment
            {
                OriginAddress = dto.OriginAddress!,
                DestinationAddress = dto.DestinationAddress!,
                Weight = (double)dto.Weight,
                Status = "Beklemede",
                CustomerId = userId // Geçici ID'yi buraya verdik
            };

            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShipments), new { id = shipment.Id }, new ShipmentResponseDto
            {
                Id = shipment.Id,
                OriginAddress = shipment.OriginAddress,
                DestinationAddress = shipment.DestinationAddress,
                Weight = shipment.Weight,
                Status = shipment.Status
            });
        }
    }
}