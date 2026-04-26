namespace LojistikAPI.Services
{
    // Sistemin her yerinden çağrılabilecek E-Posta atma aracı
    public class EmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            // GERÇEK SENARYO: Burada SmtpClient veya SendGrid kütüphanesi kullanılarak gerçek mail atılır.
            // Şimdilik geliştirme aşamasında olduğumuz için mailleri terminal ekranına (Console) yazdırıyoruz:

            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"[E-POSTA GÖNDERİLDİ]");
            Console.WriteLine($"Kime: {toEmail}");
            Console.WriteLine($"Konu: {subject}");
            Console.WriteLine($"Mesaj: {message}");
            Console.WriteLine("-------------------------------------------------");

            await Task.CompletedTask; // İşlem bitti
        }
    }
}