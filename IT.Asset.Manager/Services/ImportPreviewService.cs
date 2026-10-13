using IT.Asset.Manager.Data;
using OfficeOpenXml;
using System.IO;

namespace IT.Asset.Manager.Services;

public class ImportPreviewService
{
    public List<global::IT.Asset.Manager.Models.ImportPreviewItem> Preview(
        string filePath)
    {
        ExcelPackage.License.SetNonCommercialPersonal("Akos");

        using var package =
            new ExcelPackage(new FileInfo(filePath));

        var sheet = package.Workbook.Worksheets[0];

        var result =
            new List<global::IT.Asset.Manager.Models.ImportPreviewItem>();

        if (sheet.Dimension == null)
        {
            return result;
        }

        using var db = new AppDbContext();

        for (int row = 2; row <= sheet.Dimension.End.Row; row++)
        {
            string manufacturer =
                sheet.Cells[row, 1].Text.Trim();

            string model =
                sheet.Cells[row, 2].Text.Trim();

            string serial =
                sheet.Cells[row, 3].Text.Trim();

            string status =
                sheet.Cells[row, 4].Text.Trim();

            var item =
                new global::IT.Asset.Manager.Models.ImportPreviewItem
                {
                    RowNumber = row,
                    Manufacturer = manufacturer,
                    Model = model,
                    SerialNumber = serial,
                    Status = string.IsNullOrWhiteSpace(status)
                        ? "In Stock"
                        : status
                };

            if (string.IsNullOrWhiteSpace(serial))
            {
                item.Result = "Error";
                item.Message =
                    "Hiányzó Serial Number.";
            }
            else if (string.IsNullOrWhiteSpace(manufacturer))
            {
                item.Result = "Error";
                item.Message =
                    "Hiányzó Manufacturer.";
            }
            else if (string.IsNullOrWhiteSpace(model))
            {
                item.Result = "Error";
                item.Message =
                    "Hiányzó Model.";
            }
            else if (db.Assets.Any(
                x => x.SerialNumber == serial))
            {
                item.Result = "Duplicate";
                item.Message =
                    "Ez a Serial Number már létezik.";
            }
            else if (result.Any(
                x => x.IsValid &&
                     x.SerialNumber == serial))
            {
                item.Result = "Duplicate";
                item.Message =
                    "Ez a Serial Number többször szerepel az Excelben.";
            }
            else
            {
                item.Result = "Ready";
                item.Message =
                    "Importálható.";
            }

            result.Add(item);
        }

        return result;
    }

    public int Import(
        IEnumerable<global::IT.Asset.Manager.Models.ImportPreviewItem> items)
    {
        var validItems =
            items.Where(x => x.IsValid).ToList();

        if (validItems.Count == 0)
        {
            return 0;
        }

        using var db = new AppDbContext();

        /*
         * Egyszer meghatározzuk a legnagyobb Inventory ID-t.
         *
         * Így egy import során például:
         *
         * INV-0042
         * INV-0043
         * INV-0044
         *
         * jön létre, és nem ugyanaz az ID többször.
         */
        int highest = 0;

        foreach (var id in db.Assets
                     .Select(x => x.InventoryId))
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                continue;
            }

            if (!id.StartsWith("INV-"))
            {
                continue;
            }

            if (int.TryParse(
                id.Substring(4),
                out int number))
            {
                if (number > highest)
                {
                    highest = number;
                }
            }
        }

        int imported = 0;

        var importedAssets =
            new List<global::IT.Asset.Manager.Models.Asset>();

        foreach (var item in validItems)
        {
            if (db.Assets.Any(
                x => x.SerialNumber == item.SerialNumber))
            {
                continue;
            }

            highest++;

            var asset =
                new global::IT.Asset.Manager.Models.Asset
                {
                    InventoryId =
                        $"INV-{highest:0000}",

                    Manufacturer =
                        item.Manufacturer,

                    Model =
                        item.Model,

                    SerialNumber =
                        item.SerialNumber,

                    Status =
                        item.Status,

                    PurchaseDate =
                        DateTime.Today,

                    AssignedUser =
                        "",

                    Location =
                        "",

                    Department =
                        "",

                    Notes =
                        ""
                };

            db.Assets.Add(asset);

            importedAssets.Add(asset);

            imported++;
        }

        db.SaveChanges();

        foreach (var asset in importedAssets)
        {
            db.AssetHistories.Add(
                new global::IT.Asset.Manager.Models.AssetHistory
                {
                    AssetId =
                        asset.Id,

                    Manufacturer =
                        asset.Manufacturer,

                    Model =
                        asset.Model,

                    SerialNumber =
                        asset.SerialNumber,

                    Action =
                        "Imported",

                    Details =
                        $"Imported from Excel: " +
                        $"{asset.Manufacturer} " +
                        $"{asset.Model} " +
                        $"(Inventory ID: {asset.InventoryId}, " +
                        $"Serial: {asset.SerialNumber})",

                    Timestamp =
                        DateTime.Now
                });
        }

        db.SaveChanges();

        return imported;
    }
}