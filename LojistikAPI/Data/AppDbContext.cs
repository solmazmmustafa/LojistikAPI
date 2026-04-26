using LojistikAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace LojistikAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentDocument> ShipmentDocuments { get; set; }
        public DbSet<FuelPrice> FuelPrices { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Tracking> Trackings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SQL Server'ın kafası karışmasın diye otomatik silme (Cascade) zincirlerini kırıyoruz (Restrict yapıyoruz).

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Shipment)
                .WithMany()
                .HasForeignKey(o => o.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Company)
                .WithMany()
                .HasForeignKey(o => o.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Shipment)
                .WithMany()
                .HasForeignKey(r => r.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}