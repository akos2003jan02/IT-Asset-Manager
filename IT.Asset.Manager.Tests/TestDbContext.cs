using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace IT.Asset.Manager.Tests;

public class TestDbContext : DbContext
{
    private readonly SqliteConnection _connection;

    public DbSet<global::IT.Asset.Manager.Models.Asset> Assets
        => Set<global::IT.Asset.Manager.Models.Asset>();

    public DbSet<global::IT.Asset.Manager.Models.AssetHistory> AssetHistories
        => Set<global::IT.Asset.Manager.Models.AssetHistory>();

    public TestDbContext(SqliteConnection connection)
    {
        _connection = connection;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connection);
    }
}