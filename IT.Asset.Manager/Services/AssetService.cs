using Microsoft.EntityFrameworkCore;
using IT.Asset.Manager.Data;

namespace IT.Asset.Manager.Services;

public class AssetService
{
    private readonly Func<DbContext> contextFactory;

    public AssetService()
    {
        contextFactory = () => new AppDbContext();
    }

    public AssetService(Func<DbContext> contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public void Add(
        string manufacturer,
        string model,
        string serialNumber,
        string status)
    {
        using var db = contextFactory();

        db.Set<global::IT.Asset.Manager.Models.Asset>().Add(
            new global::IT.Asset.Manager.Models.Asset
            {
                Manufacturer = manufacturer,
                Model = model,
                SerialNumber = serialNumber,
                Status = status
            });

        db.SaveChanges();
    }

    public void Update(
        int id,
        string manufacturer,
        string model,
        string serialNumber,
        string status)
    {
        using var db = contextFactory();

        var asset = db.Set<global::IT.Asset.Manager.Models.Asset>()
            .FirstOrDefault(x => x.Id == id);

        if (asset == null)
        {
            return;
        }

        asset.Manufacturer = manufacturer;
        asset.Model = model;
        asset.SerialNumber = serialNumber;
        asset.Status = status;

        db.SaveChanges();
    }

    public void Delete(int id)
    {
        using var db = contextFactory();

        var asset = db.Set<global::IT.Asset.Manager.Models.Asset>()
            .FirstOrDefault(x => x.Id == id);

        if (asset == null)
        {
            return;
        }

        db.Set<global::IT.Asset.Manager.Models.AssetHistory>().Add(
            new global::IT.Asset.Manager.Models.AssetHistory
            {
                AssetId = asset.Id,
                Manufacturer = asset.Manufacturer,
                Model = asset.Model,
                SerialNumber = asset.SerialNumber,
                Action = "Deleted",
                Details =
                    $"Deleted asset: {asset.Manufacturer} " +
                    $"{asset.Model} " +
                    $"(Serial: {asset.SerialNumber})",
                Timestamp = DateTime.Now
            });

        db.Set<global::IT.Asset.Manager.Models.Asset>().Remove(asset);

        db.SaveChanges();
    }

    public bool Restore(
        global::IT.Asset.Manager.Models.AssetHistory history)
    {
        using var db = contextFactory();

        bool serialExists =
            db.Set<global::IT.Asset.Manager.Models.Asset>()
                .Any(x => x.SerialNumber == history.SerialNumber);

        if (serialExists)
        {
            return false;
        }

        var restoredAsset =
            new global::IT.Asset.Manager.Models.Asset
            {
                Manufacturer = history.Manufacturer,
                Model = history.Model,
                SerialNumber = history.SerialNumber,
                Status = "In Stock",
                PurchaseDate = DateTime.Today,
                AssignedUser = "",
                Notes = ""
            };

        db.Set<global::IT.Asset.Manager.Models.Asset>()
            .Add(restoredAsset);

        db.SaveChanges();

        db.Set<global::IT.Asset.Manager.Models.AssetHistory>().Add(
            new global::IT.Asset.Manager.Models.AssetHistory
            {
                AssetId = restoredAsset.Id,
                Manufacturer = restoredAsset.Manufacturer,
                Model = restoredAsset.Model,
                SerialNumber = restoredAsset.SerialNumber,
                Action = "Restored",
                Details =
                    $"Restored asset: " +
                    $"{restoredAsset.Manufacturer} " +
                    $"{restoredAsset.Model} " +
                    $"(Serial: {restoredAsset.SerialNumber})",
                Timestamp = DateTime.Now
            });

        db.SaveChanges();

        return true;
    }
}