namespace LojistikAPI.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int Score { get; set; }                      // 1 ile 5 arası puan
        public string Comment { get; set; } = string.Empty; // Yapılan yorum

        // Hangi taşıma işi için puan verildi?
        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; } = null!;

        // Puanı veren kim?
        public int FromUserId { get; set; }

        // Puanı alan kim?
        public int ToUserId { get; set; }
    }
}