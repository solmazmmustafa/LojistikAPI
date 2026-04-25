using Microsoft.AspNetCore.Mvc;

namespace LojistikAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricingController : ControllerBase
    {
        // GET: api/Pricing/calculate
        [HttpGet("calculate")]
        public IActionResult CalculatePrice(double mesafeKm, double kmBasiUcret, double ortalamaTuketim, double guncelMazotFiyati, double sabitGiderler)
        {
            // Not: Tüketim genelde "100 km'de yakılan litre" olarak hesaplanır (Örn: TIR için 100 km'de 35 litre).
            // Bu yüzden mesafeyi 100'e bölüp tüketimle çarpıyoruz.

            double yakitMaliyeti = (mesafeKm / 100) * ortalamaTuketim * guncelMazotFiyati;
            double tabanUcret = mesafeKm * kmBasiUcret;

            // Raporundaki formülün tam karşılığı:
            double toplamFiyat = tabanUcret + yakitMaliyeti + sabitGiderler;

            // Sonucu sadece tek bir rakam olarak değil, faturaya dökülmüş şık bir JSON formatında geri döndürüyoruz
            var hesapOzeti = new
            {
                RotaMesafesi = mesafeKm + " km",
                HizmetBedeli = Math.Round(tabanUcret, 2) + " TL",
                HesaplananYakitMaliyeti = Math.Round(yakitMaliyeti, 2) + " TL",
                EkGiderler = sabitGiderler + " TL",
                GenelToplam = Math.Round(toplamFiyat, 2) + " TL",
                KdvDahilToplam = Math.Round(toplamFiyat * 1.20, 2) + " TL" // Güncel %20 KDV ekledik
            };

            return Ok(hesapOzeti);
        }
    }
}