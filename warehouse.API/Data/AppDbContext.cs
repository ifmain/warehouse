using Microsoft.EntityFrameworkCore;
using warehouse.Shared.Models;

namespace warehouse.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<DatabaseItem> Items { get; set; }
    public DbSet<DatabaseCountry> Countries { get; set; }
    public DbSet<DatabaseManufacturer> Manufacturers { get; set; }
    public DbSet<DatabaseAppliances> Appliances { get; set; }
    public DbSet<DatabaseFilemanager> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<DatabaseItem>()
            .HasOne(i => i.Country).WithMany().HasForeignKey(i => i.CountryID);

        modelBuilder.Entity<DatabaseItem>()
            .HasOne(i => i.Manufacturer).WithMany().HasForeignKey(i => i.ManufacturerID);

        modelBuilder.Entity<DatabaseItem>()
            .HasOne(i => i.Appliance).WithMany().HasForeignKey(i => i.AppliancesID);

        modelBuilder.Entity<DatabaseItem>()
            .HasOne(i => i.Image).WithMany().HasForeignKey(i => i.ImageID);
            
        modelBuilder.Entity<DatabaseManufacturer>()
            .HasOne(m => m.Country).WithMany().HasForeignKey(m => m.CountryID);
    }
}