using System.Windows;
using System.Windows.Controls;
using IT.Asset.Manager.Data;
using IT.Asset.Manager.Services;
namespace IT.Asset.Manager.Views;

public partial class AddAssetWindow : Window
{
    private readonly global::IT.Asset.Manager.Models.Asset? editingAsset;

    public AddAssetWindow(
        global::IT.Asset.Manager.Models.Asset? asset = null)
    {
        InitializeComponent();

        editingAsset = asset;

        if (asset != null)
        {
            Title = "Edit Device";
            TitleText.Text = "Edit Device";

            InventoryIdBox.Text = asset.InventoryId;

            ManufacturerBox.Text = asset.Manufacturer;
            ModelBox.Text = asset.Model;
            SerialBox.Text = asset.SerialNumber;

            PurchaseDatePicker.SelectedDate =
                asset.PurchaseDate;

            AssignedUserBox.Text =
                asset.AssignedUser;

            LocationBox.Text =
                asset.Location;

            DepartmentBox.Text =
                asset.Department;

            NotesBox.Text =
                asset.Notes;

            foreach (ComboBoxItem item in StatusBox.Items)
            {
                if (item.Content?.ToString() == asset.Status)
                {
                    StatusBox.SelectedItem = item;
                    break;
                }
            }
        }
        else
        {
            PurchaseDatePicker.SelectedDate =
                DateTime.Today;

            InventoryIdBox.Text =

                InventoryIdService.GenerateNextId();
        }
    }

    private void Save_Click(
        object sender,
        RoutedEventArgs e)
    {
        string manufacturer =
            ManufacturerBox.Text.Trim();

        string model =
            ModelBox.Text.Trim();

        string serial =
            SerialBox.Text.Trim();

        string status =
            (StatusBox.SelectedItem as ComboBoxItem)
                ?.Content?.ToString()
            ?? "In Stock";

        DateTime purchaseDate =
            PurchaseDatePicker.SelectedDate
            ?? DateTime.Today;

        string assignedUser =
            AssignedUserBox.Text.Trim();

        string location =
            LocationBox.Text.Trim();

        string department =
            DepartmentBox.Text.Trim();

        string notes =
            NotesBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(manufacturer) ||
            string.IsNullOrWhiteSpace(model) ||
            string.IsNullOrWhiteSpace(serial))
        {
            MessageBox.Show(
                "A Manufacturer, Model és Serial Number mezők kitöltése kötelező!",
                "Hiányzó adatok",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        using var db = new AppDbContext();

        bool serialExists =
            editingAsset == null
                ? db.Assets.Any(
                    x => x.SerialNumber == serial)
                : db.Assets.Any(
                    x =>
                        x.SerialNumber == serial &&
                        x.Id != editingAsset.Id);

        if (serialExists)
        {
            MessageBox.Show(
                "Ez a serial number már létezik!",
                "Duplikált serial",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        if (editingAsset == null)
        {
            string inventoryId =
                InventoryIdService.GenerateNextId();

            var newAsset =
                new global::IT.Asset.Manager.Models.Asset
                {
                    InventoryId = inventoryId,
                    Manufacturer = manufacturer,
                    Model = model,
                    SerialNumber = serial,
                    Status = status,
                    PurchaseDate = purchaseDate,
                    AssignedUser = assignedUser,
                    Location = location,
                    Department = department,
                    Notes = notes
                };

            db.Assets.Add(newAsset);

            db.SaveChanges();

            db.AssetHistories.Add(
                new global::IT.Asset.Manager.Models.AssetHistory
                {
                    AssetId = newAsset.Id,
                    Manufacturer = newAsset.Manufacturer,
                    Model = newAsset.Model,
                    SerialNumber = newAsset.SerialNumber,
                    Action = "Created",
                    Details =
                        $"Asset created: " +
                        $"{manufacturer} {model} " +
                        $"(Inventory ID: {inventoryId}, " +
                        $"Serial: {serial})",
                    Timestamp = DateTime.Now
                });

            db.SaveChanges();
        }
        else
        {
            var asset =
                db.Assets.FirstOrDefault(
                    x => x.Id == editingAsset.Id);

            if (asset == null)
            {
                MessageBox.Show(
                    "Az eszköz nem található az adatbázisban.",
                    "Hiba",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            string oldManufacturer =
                asset.Manufacturer;

            string oldModel =
                asset.Model;

            string oldSerial =
                asset.SerialNumber;

            string oldStatus =
                asset.Status;

            DateTime oldPurchaseDate =
                asset.PurchaseDate;

            string oldAssignedUser =
                asset.AssignedUser;

            string oldLocation =
                asset.Location;

            string oldDepartment =
                asset.Department;

            string oldNotes =
                asset.Notes;

            asset.Manufacturer =
                manufacturer;

            asset.Model =
                model;

            asset.SerialNumber =
                serial;

            asset.Status =
                status;

            asset.PurchaseDate =
                purchaseDate;

            asset.AssignedUser =
                assignedUser;

            asset.Location =
                location;

            asset.Department =
                department;

            asset.Notes =
                notes;

            db.SaveChanges();

            if (oldManufacturer != manufacturer)
            {
                AddHistory(
                    db,
                    asset,
                    "Manufacturer changed",
                    $"{oldManufacturer} → {manufacturer}");
            }

            if (oldModel != model)
            {
                AddHistory(
                    db,
                    asset,
                    "Model changed",
                    $"{oldModel} → {model}");
            }

            if (oldSerial != serial)
            {
                AddHistory(
                    db,
                    asset,
                    "Serial Number changed",
                    $"{oldSerial} → {serial}");
            }

            if (oldStatus != status)
            {
                AddHistory(
                    db,
                    asset,
                    "Status changed",
                    $"{oldStatus} → {status}");
            }

            if (oldPurchaseDate != purchaseDate)
            {
                AddHistory(
                    db,
                    asset,
                    "Purchase Date changed",
                    $"{oldPurchaseDate:yyyy.MM.dd} → " +
                    $"{purchaseDate:yyyy.MM.dd}");
            }

            if (oldAssignedUser != assignedUser)
            {
                AddHistory(
                    db,
                    asset,
                    "Assigned User changed",
                    $"{DisplayValue(oldAssignedUser)} → " +
                    $"{DisplayValue(assignedUser)}");
            }

            if (oldLocation != location)
            {
                AddHistory(
                    db,
                    asset,
                    "Location changed",
                    $"{DisplayValue(oldLocation)} → " +
                    $"{DisplayValue(location)}");
            }

            if (oldDepartment != department)
            {
                AddHistory(
                    db,
                    asset,
                    "Department changed",
                    $"{DisplayValue(oldDepartment)} → " +
                    $"{DisplayValue(department)}");
            }

            if (oldNotes != notes)
            {
                AddHistory(
                    db,
                    asset,
                    "Notes changed",
                    $"{DisplayValue(oldNotes)} → " +
                    $"{DisplayValue(notes)}");
            }

            db.SaveChanges();
        }

        DialogResult = true;
        Close();
    }

    private static void AddHistory(
        AppDbContext db,
        global::IT.Asset.Manager.Models.Asset asset,
        string action,
        string details)
    {
        db.AssetHistories.Add(
            new global::IT.Asset.Manager.Models.AssetHistory
            {
                AssetId = asset.Id,
                Manufacturer = asset.Manufacturer,
                Model = asset.Model,
                SerialNumber = asset.SerialNumber,
                Action = action,
                Details = details,
                Timestamp = DateTime.Now
            });
    }

    private static string DisplayValue(
        string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "(empty)"
            : value;
    }

}