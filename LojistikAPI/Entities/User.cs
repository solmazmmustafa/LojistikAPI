namespace LojistikAPI.Entities
{
    // Sistemdeki tüm kullanıcıları (Müşteri, Firma, Admin) tutacağımız tablo
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Müşteri" veya "Firma"
        public string PhoneNumber { get; set; }
    }
}