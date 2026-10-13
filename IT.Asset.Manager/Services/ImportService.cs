using IT.Asset.Manager.Data;
using OfficeOpenXml;
using System.IO;

namespace IT.Asset.Manager.Services;

public class ImportService
{
    public int Import(string filePath)
    {
        ExcelPackage.License.SetNonCommercialPersonal("Akos");

        using var package = new ExcelPackage(new FileInfo(filePath));
        var sheet = package.Workbook.Worksheets[0];

        if (sheet.Dimension == null)
            return 0;

        using var db = new AppDbContext();

        int imported = 0;

        for (int row = 2; row <= sheet.Dimension.End.Row; row++)
        {
            string manufacturer = sheet.Cells[row, 1].Text.Trim();
            string model = sheet.Cells[row, 2].Text.Trim();
            string serial = sheet.Cells[row, 3].Text.Trim();
            string status = sheet.Cells[row, 4].Text.Trim();

            if (string.IsNullOrWhiteSpace(serial))
                continue;

            if (db.Assets.Any(x => x.SerialNumber == serial))
                continue;

            db.Assets.Add(new global::IT.Asset.Manager.Models.Asset
            {
                Manufacturer = manufacturer,
                Model = model,
                SerialNumber = serial,
                Status = status
            });

            imported++;
        }

        db.SaveChanges();
        return imported;
    }
}