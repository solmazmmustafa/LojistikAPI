using LojistikAPI.Features.Shipments.Commands;
using LojistikAPI.Features.Shipments.Queries;
using LojistikAPI.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LojistikAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        // Sadece Postacıyı (MediatR) içeri alıyoruz. DbContext'e elveda!
        private readonly IMediator _mediator;

        public ShipmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetShipments([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // İsteği zarfa koy ve postacıya ver
            var query = new GetShipmentsQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentDto dto)
        {
            // Kullanıcı ID'sini Token'dan al
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Lütfen giriş yapınız veya geçerli bir token kullanınız." });

            // İsteği zarfa koy ve postacıya ver
            var command = new CreateShipmentCommand(dto, userId);
            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetShipments), new { id = result.Id }, result);
        }
    }
}