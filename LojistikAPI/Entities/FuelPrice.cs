using System;

namespace LojistikAPI.Entities
{
    public class FuelPrice
    {
        public int Id { get; set; }
        public double PricePerLiter { get; set; }             // Litre fiyatı
        public string FuelType { get; set; } = "Motorin";     // Yakıt türü
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow; // Sisteme kaydedildiği an
    }
}