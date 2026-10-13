using Microsoft.Data.Sqlite;

namespace IT.Asset.Manager.Tests;

public class AssetTests
{
    private SqliteConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var db = new TestDbContext(connection);
        db.Database.EnsureCreated();
        return connection;
    }

    [Fact]
    public void Asset_ShouldHaveCorrectData()
    {
        var asset = new global::IT.Asset.Manager.Models.Asset
        {
            InventoryId = "INV-0099",
            Manufacturer = "Dell",
            Model = "Latitude 5420",
            SerialNumber = "TEST-SERIAL-001",
            Status = "In Stock",
            AssignedUser = "Test User",
            Location = "Budapest",
            Department = "IT"
        };

        Assert.Equal("INV-0099", asset.InventoryId);
        Assert.Equal("Dell", asset.Manufacturer);
        Assert.Equal("Latitude 5420", asset.Model);
        Assert.Equal("TEST-SERIAL-001", asset.SerialNumber);
        Assert.Equal("In Stock", asset.Status);
        Assert.Equal("Test User", asset.AssignedUser);
        Assert.Equal("Budapest", asset.Location);
        Assert.Equal("IT", asset.Department);
    }

    [Fact]
    public void Asset_ShouldBeSavedToDatabase()
    {
        using var connection = CreateOpenConnection();
        using var db = new TestDbContext(connection);

        var asset = new global::IT.Asset.Manager.Models.Asset
        {
            InventoryId = "INV-0100",
            Manufacturer = "Dell",
            Model = "Latitude 5540",
            SerialNumber = "TEST-SERIAL-002",
            Status = "In Stock"
        };

        db.Assets.Add(asset);
        db.SaveChanges();

        using var verifyDb = new TestDbContext(connection);
        var savedAsset = verifyDb.Assets.FirstOrDefault(x => x.SerialNumber == "TEST-SERIAL-002");

        Assert.NotNull(savedAsset);
        Assert.Equal("INV-0100", savedAsset.InventoryId);
        Assert.Equal("Dell", savedAsset.Manufacturer);
        Assert.Equal("Latitude 5540", savedAsset.Model);
    }

    [Fact]
    public void AssetService_Add_ShouldCreateAsset()
    {
        using var connection = CreateOpenConnection();
        var service = new global::IT.Asset.Manager.Services.AssetService(() => new TestDbContext(connection));

        service.Add("Dell", "Latitude 5420", "TEST-SERVICE-001", "In Stock");

        using var verifyDb = new TestDbContext(connection);
        var asset = verifyDb.Assets.FirstOrDefault(x => x.SerialNumber == "TEST-SERVICE-001");

        Assert.NotNull(asset);
        Assert.Equal("Dell", asset.Manufacturer);
        Assert.Equal("Latitude 5420", asset.Model);
        Assert.Equal("In Stock", asset.Status);
    }

    [Fact]
    public void AssetService_Update_ShouldChangeAsset()
    {
        using var connection = CreateOpenConnection();
        int assetId;

        using (var setupDb = new TestDbContext(connection))
        {
            var asset = new global::IT.Asset.Manager.Models.Asset
            {
                Manufacturer = "Dell",
                Model = "Latitude 5420",
                SerialNumber = "TEST-SERVICE-002",
                Status = "In Stock"
            };
            setupDb.Assets.Add(asset);
            setupDb.SaveChanges();
            assetId = asset.Id;
        }

        var service = new global::IT.Asset.Manager.Services.AssetService(() => new TestDbContext(connection));
        service.Update(assetId, "Lenovo", "ThinkPad T14", "TEST-SERVICE-003", "Issued");

        using var verifyDb = new TestDbContext(connection);
        var updatedAsset = verifyDb.Assets.FirstOrDefault(x => x.Id == assetId);

        Assert.NotNull(updatedAsset);
        Assert.Equal("Lenovo", updatedAsset.Manufacturer);
        Assert.Equal("ThinkPad T14", updatedAsset.Model);
        Assert.Equal("TEST-SERVICE-003", updatedAsset.SerialNumber);
        Assert.Equal("Issued", updatedAsset.Status);
    }

    [Fact]
    public void AssetService_Delete_ShouldRemoveAssetAndCreateHistory()
    {
        using var connection = CreateOpenConnection();
        int assetId;

        using (var setupDb = new TestDbContext(connection))
        {
            var asset = new global::IT.Asset.Manager.Models.Asset
            {
                Manufacturer = "HP",
                Model = "EliteBook 840",
                SerialNumber = "TEST-SERVICE-004",
                Status = "In Stock"
            };
            setupDb.Assets.Add(asset);
            setupDb.SaveChanges();
            assetId = asset.Id;
        }

        var service = new global::IT.Asset.Manager.Services.AssetService(() => new TestDbContext(connection));
        service.Delete(assetId);

        using var verifyDb = new TestDbContext(connection);
        var deletedAsset = verifyDb.Assets.FirstOrDefault(x => x.Id == assetId);
        var history = verifyDb.AssetHistories.FirstOrDefault(x => x.AssetId == assetId && x.Action == "Deleted");

        Assert.Null(deletedAsset);
        Assert.NotNull(history);
        Assert.Equal("HP", history.Manufacturer);
        Assert.Equal("EliteBook 840", history.Model);
        Assert.Equal("TEST-SERVICE-004", history.SerialNumber);
    }

    [Fact]
    public void AssetService_Restore_ShouldRestoreDeletedAsset()
    {
        using var connection = CreateOpenConnection();

        var history = new global::IT.Asset.Manager.Models.AssetHistory
        {
            AssetId = 123,
            Manufacturer = "Lenovo",
            Model = "ThinkPad T14",
            SerialNumber = "TEST-SERVICE-005",
            Action = "Deleted",
            Details = "Deleted asset"
        };

        var service = new global::IT.Asset.Manager.Services.AssetService(() => new TestDbContext(connection));
        bool result = service.Restore(history);

        using var verifyDb = new TestDbContext(connection);
        var restoredAsset = verifyDb.Assets.FirstOrDefault(x => x.SerialNumber == "TEST-SERVICE-005");
        var restoreHistory = verifyDb.AssetHistories.FirstOrDefault(x => x.SerialNumber == "TEST-SERVICE-005" && x.Action == "Restored");

        Assert.True(result);
        Assert.NotNull(restoredAsset);
        Assert.Equal("Lenovo", restoredAsset.Manufacturer);
        Assert.Equal("ThinkPad T14", restoredAsset.Model);
        Assert.Equal("In Stock", restoredAsset.Status);
        Assert.NotNull(restoreHistory);
    }

    [Fact]
    public void AssetService_Restore_ShouldFailIfSerialAlreadyExists()
    {
        using var connection = CreateOpenConnection();

        using (var setupDb = new TestDbContext(connection))
        {
            setupDb.Assets.Add(new global::IT.Asset.Manager.Models.Asset
            {
                Manufacturer = "Dell",
                Model = "Latitude 5540",
                SerialNumber = "TEST-SERVICE-006",
                Status = "In Stock"
            });
            setupDb.SaveChanges();
        }

        var history = new global::IT.Asset.Manager.Models.AssetHistory
        {
            AssetId = 999,
            Manufacturer = "Dell",
            Model = "Latitude 5540",
            SerialNumber = "TEST-SERVICE-006",
            Action = "Deleted",
            Details = "Deleted asset"
        };

        var service = new global::IT.Asset.Manager.Services.AssetService(() => new TestDbContext(connection));
        bool result = service.Restore(history);

        Assert.False(result);

        using var verifyDb = new TestDbContext(connection);
        int assetCount = verifyDb.Assets.Count(x => x.SerialNumber == "TEST-SERVICE-006");
        Assert.Equal(1, assetCount);
    }
}