using System;

namespace LojistikAPI.Entities
{
    // Müşterilerin oluşturduğu yük ilanlarını tutacağımız tablo
    public class Shipment
    {
        public int Id { get; set; }
        public string OriginAddress { get; set; }      // Çıkış Adresi
        public string DestinationAddress { get; set; } // Varış Adresi
        public double Weight { get; set; }             // Ağırlık (Ton)
        public string Status { get; set; }             // Beklemede, Yükte, Teslim Edildi

        // İlanı hangi müşterinin açtığını tutan ilişki
        public int CustomerId { get; set; }
        public User? Customer { get; set; }
    }
}