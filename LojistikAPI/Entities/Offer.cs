using System;

namespace LojistikAPI.Entities
{
    public class Offer
    {
        public int Id { get; set; }
        public double Price { get; set; }                   // Verilen fiyat teklifi
        public DateTime ValidUntil { get; set; }            // Teklifin geçerlilik süresi
        public string Status { get; set; } = "Beklemede";   // Beklemede, Kabul Edildi, Reddedildi

        // Hangi ilana teklif verildi?
        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; } = null!;

        // Hangi firma teklif verdi?
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
    }
}