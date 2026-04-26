using LojistikAPI.Data;
using LojistikAPI.Entities;
using LojistikAPI.Models;              // ← EKLENEN SATIR — CS0246 çözümü
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });

            var shipment = new Shipment
            {
                OriginAddress = dto.OriginAddress!,
                DestinationAddress = dto.DestinationAddress!,
                Weight = dto.Weight,
                Status = "Beklemede",
                CustomerId = userId
            };

            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShipments), new { id = shipment.Id }, new
            {
                shipment.Id,
                shipment.OriginAddress,
                shipment.DestinationAddress,
                shipment.Weight,
                shipment.Status,
                shipment.CustomerId
            });
        }
    }
}