using System;

namespace LojistikAPI.Entities
{
    public class Shipment
    {
        public int Id { get; set; }
        public string OriginAddress { get; set; } = string.Empty;
        public string DestinationAddress { get; set; } = string.Empty;
        public double Weight { get; set; }
        public string Status { get; set; } = string.Empty;

        public int CustomerId { get; set; }
        public User? Customer { get; set; }
    }
}