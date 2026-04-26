using System.ComponentModel.DataAnnotations;

namespace LojistikAPI.Models   // ← namespace kritik
{
    public class LoginModel
    {
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string? Password { get; set; }
    }
}