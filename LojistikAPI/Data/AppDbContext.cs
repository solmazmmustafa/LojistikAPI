using LojistikAPI.Entities; // Kendi sınıflarımızı kullanmak için
using Microsoft.EntityFrameworkCore; // EF Core kütüphanesi

namespace LojistikAPI.Data
{
    // Bu sınıf Entity Framework'ün kalbidir. Tüm tablolarımızı burada tanımlarız.
    public class AppDbContext : DbContext
    {
        // Bu constructor (yapıcı metot), bağlantı ayarlarımızı dışarıdan almamızı sağlar
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Aşağıdaki her bir DbSet, veritabanında bir TABLO'ya karşılık gelir.
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
    }
}