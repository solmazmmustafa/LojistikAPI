using System.ComponentModel.DataAnnotations;

namespace LojistikAPI.Models   // ← namespace kritik
{
    public class CreateShipmentDto
    {
        [Required(ErrorMessage = "Yükleme adresi zorunludur.")]
        [StringLength(250)]
        public string? OriginAddress { get; set; }

        [Required(ErrorMessage = "Teslimat adresi zorunludur.")]
        [StringLength(250)]
        public string? DestinationAddress { get; set; }

        [Required]
        [Range(0.01, 99999.99)]
        public decimal Weight { get; set; }
    }
}