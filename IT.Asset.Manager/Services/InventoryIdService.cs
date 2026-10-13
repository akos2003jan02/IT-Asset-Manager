using IT.Asset.Manager.Data;

namespace IT.Asset.Manager.Services;

public static class InventoryIdService
{
    public static string GenerateNextId()
    {
        using var db = new AppDbContext();

        int highest = 0;

        foreach (var id in db.Assets.Select(x => x.InventoryId))
        {
            if (string.IsNullOrWhiteSpace(id))
                continue;

            if (!id.StartsWith("INV-"))
                continue;

            if (int.TryParse(id.Substring(4), out int number))
            {
                if (number > highest)
                    highest = number;
            }
        }

        return $"INV-{highest + 1:0000}";
    }
}