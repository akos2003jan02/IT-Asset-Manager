using IT.Asset.Manager.Data;

namespace IT.Asset.Manager.Services;

public class AssetHistoryService
{
    public void Add(
        int assetId,
        string action,
        string details)
    {
        using var db = new AppDbContext();

        db.AssetHistories.Add(
            new global::IT.Asset.Manager.Models.AssetHistory
            {
                AssetId = assetId,
                Action = action,
                Details = details,
                Timestamp = DateTime.Now
            });

        db.SaveChanges();
    }

    public List<global::IT.Asset.Manager.Models.AssetHistory> GetForAsset(
        int assetId)
    {
        using var db = new AppDbContext();

        return db.AssetHistories
            .Where(x => x.AssetId == assetId)
            .OrderByDescending(x => x.Timestamp)
            .ToList();
    }

    public List<global::IT.Asset.Manager.Models.AssetHistory> GetAll()
    {
        using var db = new AppDbContext();

        return db.AssetHistories
            .OrderByDescending(x => x.Timestamp)
            .ToList();
    }
}