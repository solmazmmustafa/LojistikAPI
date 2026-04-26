using LojistikAPI.Data;
using LojistikAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojistikAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricingController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Veritabanı bağlantısını Fiyat Motoruna enjekte ediyoruz
        public PricingController(AppDbContext context)
        {
            _context = context;
        }

        private readonly Dictionary<string, double> _distances = new(StringComparer.OrdinalIgnoreCase)
        {
            { "İstanbul-Ankara", 450 }, { "Ankara-İstanbul", 450 },
            { "İstanbul-İzmir", 480 }, { "İzmir-İstanbul", 480 }
        };

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculatePrice([FromBody] CalculatePriceDto dto)
        {
            string route = $"{dto.OriginCity}-{dto.DestinationCity}";

            if (!_distances.TryGetValue(route, out double distanceKm))
                distanceKm = 500;

            double fuelConsumptionPer100Km = dto.VehicleType.ToUpper() switch
            {
                "KAMYONET" => 12,
                "KAMYON" => 25,
                _ => 35 // TIR
            };

            double basePricePerKm = dto.VehicleType.ToUpper() switch
            {
                "KAMYONET" => 15,
                "KAMYON" => 25,
                _ => 40 // TIR
            };

            // ✅ İŞTE AKILLI KISIM: Veritabanındaki "En Son Eklenen" mazot fiyatını buluyoruz
            var latestFuelRecord = await _context.FuelPrices
                .OrderByDescending(f => f.RecordedAt)
                .FirstOrDefaultAsync();

            // Eğer henüz veri çekilmediyse güvenlik amaçlı varsayılan 42.50 kullanıyoruz
            double currentDieselPrice = latestFuelRecord != null ? latestFuelRecord.PricePerLiter : 42.50;

            double totalFuelNeeded = (distanceKm / 100) * fuelConsumptionPer100Km;
            double fuelCost = totalFuelNeeded * currentDieselPrice;
            double transportCost = distanceKm * basePricePerKm;

            double subTotal = transportCost + fuelCost + dto.AdditionalCosts;
            double totalWithTax = subTotal * 1.20;

            var result = new
            {
                Rota = route,
                Mesafe = distanceKm + " km",
                AracTipi = dto.VehicleType.ToUpper(),
                GuncelMazotFiyati = currentDieselPrice + " TL/Litre", // Müşteriye şeffaf fiyat sunumu
                HesaplananYakit = Math.Round(fuelCost, 2) + " TL",
                HizmetBedeli = Math.Round(transportCost, 2) + " TL",
                EkGiderler = dto.AdditionalCosts + " TL",
                AraToplam = Math.Round(subTotal, 2) + " TL",
                KdvDahilToplam = Math.Round(totalWithTax, 2) + " TL"
            };

            return Ok(result);
        }
    }
}