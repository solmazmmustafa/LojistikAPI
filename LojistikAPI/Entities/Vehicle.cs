namespace LojistikAPI.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string PlateNo { get; set; } = string.Empty; // Plaka
        public string Type { get; set; } = string.Empty;    // Örn: TIR, Kamyonet
        public double Capacity { get; set; }                // Taşıma kapasitesi (Ton)

        // Bu araç hangi firmaya ait? (İlişki)
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
    }
}