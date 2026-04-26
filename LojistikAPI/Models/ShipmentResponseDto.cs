namespace LojistikAPI.Models
{
    public class ShipmentResponseDto
    {
        public int Id { get; set; }
        public string? OriginAddress { get; set; }      // "?" null uyarılarını çözer
        public string? DestinationAddress { get; set; }
        public double Weight { get; set; }              // CS0117 hatası için eklendi
        public string? Status { get; set; }
    }
}