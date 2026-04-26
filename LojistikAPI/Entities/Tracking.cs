using System;

namespace LojistikAPI.Entities
{
    public class Tracking
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }                   // Hangi ilanın/yükün konumu?
        public double Latitude { get; set; }                  // Enlem (Örn: 41.0082)
        public double Longitude { get; set; }                 // Boylam (Örn: 28.9784)
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Konumun alındığı o anki zaman

        public Shipment? Shipment { get; set; }
    }
}