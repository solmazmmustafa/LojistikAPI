using System.ComponentModel.DataAnnotations;

namespace LojistikAPI.Models
{
    // Fiyat hesaplamak için müşteriden isteyeceğimiz bilgilerin şablonu
    public class CalculatePriceDto
    {
        [Required(ErrorMessage = "Çıkış şehri zorunludur.")]
        public string OriginCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Varış şehri zorunludur.")]
        public string DestinationCity { get; set; } = string.Empty;

        // Kullanıcı boş bırakırsa sistem otomatik "TIR" varsaysın
        public string VehicleType { get; set; } = "TIR";

        // Köprü, otoyol, hamaliye gibi ek masraflar
        public double AdditionalCosts { get; set; } = 0;
    }
}