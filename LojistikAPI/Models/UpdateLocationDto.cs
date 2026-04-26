namespace LojistikAPI.Models
{
    public class UpdateLocationDto
    {
        public int ShipmentId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}