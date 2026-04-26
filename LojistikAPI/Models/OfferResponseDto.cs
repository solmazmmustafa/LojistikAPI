namespace LojistikAPI.Models
{
    // Müşteriye teklifleri gösterirken kullanacağımız şık sunum şablonu
    public class OfferResponseDto
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public string Status { get; set; } = string.Empty; // Beklemede, Kabul Edildi vs.

        // Firmanın detaylarını da müşteriye gösterelim ki kimin teklif verdiğini bilsin
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }
}