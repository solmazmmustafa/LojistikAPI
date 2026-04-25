namespace LojistikAPI.Entities
{
    // Nakliye firmalarının profillerini tutacağımız tablo
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TaxNo { get; set; }     // Vergi Numarası
        public string LicenseNo { get; set; } // K1/K2 Yetki Belgesi
        public double Rating { get; set; }    // Firma puanı (örn: 4.5)

        // Bu firmanın hangi kullanıcı hesabına ait olduğunu belirten ilişki
        public int UserId { get; set; }
        public User User { get; set; }
    }
}