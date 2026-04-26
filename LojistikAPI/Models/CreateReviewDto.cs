using System.ComponentModel.DataAnnotations;

namespace LojistikAPI.Models
{
    // Müşteriden gelecek puan ve yorum bilgileri
    public class CreateReviewDto
    {
        [Required]
        public int ShipmentId { get; set; } // Hangi taşıma işi?

        [Required]
        public int ToUserId { get; set; } // Puan verilen firma/kullanıcı kim?

        [Required(ErrorMessage = "Lütfen 1 ile 5 arasında bir puan verin.")]
        [Range(1, 5)]
        public int Score { get; set; } // 1 ile 5 yıldız arası

        public string Comment { get; set; } = string.Empty; // Müşterinin yorumu
    }
}