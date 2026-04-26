using System;

namespace LojistikAPI.Entities
{
    // Yüklenen belgelerin (Fotoğraf, İrsaliye vb.) kayıtlarını tutacak tablo
    public class ShipmentDocument
    {
        public int Id { get; set; }

        // Bu belge hangi taşımaya/ilana ait?
        public int ShipmentId { get; set; }

        // Belgenin türü ne? (Örn: "İrsaliye", "Sigorta", "Yük Fotoğrafı")
        public string DocumentType { get; set; } = string.Empty;

        // Dosyanın sunucumuzdaki adresi (Örn: "/uploads/irsaliye_123.jpg")
        public string FilePath { get; set; } = string.Empty;

        // Ne zaman yüklendi?
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // İlişki bağlantısı
        public Shipment Shipment { get; set; } = null!;
    }
}