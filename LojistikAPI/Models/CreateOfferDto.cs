using System.ComponentModel.DataAnnotations;

namespace LojistikAPI.Models
{
    // Firma teklif verirken bize gelecek olan bilgilerin şablonu
    public class CreateOfferDto
    {
        [Required(ErrorMessage = "İlan ID'si zorunludur.")]
        public int ShipmentId { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(1, 1000000, ErrorMessage = "Lütfen geçerli bir fiyat giriniz.")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Teklifin geçerlilik tarihi zorunludur.")]
        public DateTime ValidUntil { get; set; }
    }
}