using Microsoft.EntityFrameworkCore;

namespace IT.Asset.Manager.Data;

public class AppDbContext : DbContext
{
    public DbSet<global::IT.Asset.Manager.Models.Asset> Assets
        => Set<global::IT.Asset.Manager.Models.Asset>();

    public DbSet<global::IT.Asset.Manager.Models.AssetHistory> AssetHistories
        => Set<global::IT.Asset.Manager.Models.AssetHistory>();

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            "Data Source=assets.db");
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<
            global::IT.Asset.Manager.Models.Asset>()
            .HasIndex(x => x.InventoryId)
            .IsUnique();

        modelBuilder.Entity<
            global::IT.Asset.Manager.Models.Asset>()
            .HasIndex(x => x.SerialNumber)
            .IsUnique();

        modelBuilder.Entity<
            global::IT.Asset.Manager.Models.Asset>()
            .HasIndex(x => x.Status);

        modelBuilder.Entity<
            global::IT.Asset.Manager.Models.Asset>()
            .HasIndex(x => x.AssignedUser);

        modelBuilder.Entity<
            global::IT.Asset.Manager.Models.Asset>()
            .HasIndex(x => x.PurchaseDate);

        modelBuilder.Entity<
            global::IT.Asset.Manager.Models.Asset>()
            .HasIndex(x => x.Manufacturer);

        modelBuilder.Entity<
            global::IT.Asset.Manager.Models.Asset>()
            .HasIndex(x => x.Model);
    }
}