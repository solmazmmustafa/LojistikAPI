namespace LojistikAPI.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TaxNo { get; set; } = string.Empty;
        public string LicenseNo { get; set; } = string.Empty;
        public double Rating { get; set; }

        public int UserId { get; set; }
        // "null!" ifadesi Entity Framework'e: "Bunu sen dolduracaksın, merak etme" demektir.
        public User User { get; set; } = null!;
    }
}