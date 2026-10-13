using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.IO;

namespace IT.Asset.Manager.Services;

public class ExportService
{
    public void Export(
        string filePath,
        IEnumerable<global::IT.Asset.Manager.Models.Asset> assets)
    {
        ExcelPackage.License.SetNonCommercialPersonal("Akos");

        using var package = new ExcelPackage();

        var sheet = package.Workbook.Worksheets.Add("Assets");

        // Fejlécek
        sheet.Cells[1, 1].Value = "Inventory ID";
        sheet.Cells[1, 2].Value = "Manufacturer";
        sheet.Cells[1, 3].Value = "Model";
        sheet.Cells[1, 4].Value = "Serial Number";
        sheet.Cells[1, 5].Value = "Location";
        sheet.Cells[1, 6].Value = "Department";
        sheet.Cells[1, 7].Value = "Status";
        sheet.Cells[1, 8].Value = "Assigned User";
        sheet.Cells[1, 9].Value = "Purchase Date";
        sheet.Cells[1, 10].Value = "Notes";

        int row = 2;

        foreach (var asset in assets)
        {
            sheet.Cells[row, 1].Value = asset.InventoryId;
            sheet.Cells[row, 2].Value = asset.Manufacturer;
            sheet.Cells[row, 3].Value = asset.Model;
            sheet.Cells[row, 4].Value = asset.SerialNumber;
            sheet.Cells[row, 5].Value = asset.Location;
            sheet.Cells[row, 6].Value = asset.Department;
            sheet.Cells[row, 7].Value = asset.Status;
            sheet.Cells[row, 8].Value = asset.AssignedUser;
            sheet.Cells[row, 9].Value = asset.PurchaseDate;
            sheet.Cells[row, 10].Value = asset.Notes;

            row++;
        }

        int lastRow = row - 1;

        // Fejléc formázása
        using (var header = sheet.Cells[1, 1, 1, 10])
        {
            header.Style.Font.Bold = true;
            header.Style.HorizontalAlignment =
                OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        }

        // Dátum formázása
        if (lastRow >= 2)
        {
            sheet.Cells[2, 9, lastRow, 9]
                .Style.Numberformat.Format = "yyyy.mm.dd";
        }

        // Excel Table létrehozása
        if (lastRow >= 2)
        {
            var tableRange =
                sheet.Cells[1, 1, lastRow, 7];

            var table =
                sheet.Tables.Add(
                    tableRange,
                    "AssetsTable");

            table.TableStyle =
                TableStyles.Medium2;
        }

        // Oszlopszélesség
        sheet.Column(1).Width = 14;
        sheet.Column(2).Width = 20;
        sheet.Column(3).Width = 24;
        sheet.Column(4).Width = 22;
        sheet.Column(5).Width = 18;
        sheet.Column(6).Width = 18;
        sheet.Column(7).Width = 14;
        sheet.Column(8).Width = 22;
        sheet.Column(9).Width = 16;
        sheet.Column(10).Width = 40;

        // Notes tördelése
        if (lastRow >= 2)
        {
            sheet.Cells[2, 10, lastRow, 10]
                .Style.WrapText = true;
        }

        // Freeze header
        sheet.View.FreezePanes(2, 1);

        package.SaveAs(new FileInfo(filePath));
    }
}