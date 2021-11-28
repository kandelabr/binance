using Microsoft.EntityFrameworkCore;

namespace Binance.DB.Models
{
    public class BinanceContext : DbContext
    {
        public DbSet<Information> Informations { get; set; }
        public DbSet<Price> Prices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=localhost\SQLEXPRESS;Database=Binance;User Id=BinanceUser; Password=Abc123!@#123Abc123!@#123;Trusted_Connection=false");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Price>().Property(e => e.Amount).HasPrecision(20, 8);
        }
    }
}
