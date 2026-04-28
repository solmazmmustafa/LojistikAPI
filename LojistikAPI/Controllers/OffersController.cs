using LojistikAPI.Models;
using LojistikAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojistikAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OffersController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOffer([FromBody] CreateOfferDto dto)
        {
            // Artık try-catch yok! Kod patlarsa Middleware havada yakalayacak.
            var offerId = await _offerService.CreateOfferAsync(dto);
            return Ok(new { message = "Teklif başarıyla gönderildi ve müşteriye bildirildi!", offerId = offerId });
        }
    }
}