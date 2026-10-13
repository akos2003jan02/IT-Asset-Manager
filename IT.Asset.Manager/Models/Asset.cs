using System.ComponentModel;

namespace IT.Asset.Manager.Models;

public class Asset : INotifyPropertyChanged
{
    public int Id { get; set; }

    private string inventoryId = "";
    private string manufacturer = "";
    private string model = "";
    private string serialNumber = "";
    private string status = "";
    private string location = "";
    private string department = "";

    public string InventoryId
    {
        get => inventoryId;
        set
        {
            inventoryId = value;
            OnPropertyChanged(nameof(InventoryId));
        }
    }

    public string Manufacturer
    {
        get => manufacturer;
        set
        {
            manufacturer = value;
            OnPropertyChanged(nameof(Manufacturer));
        }
    }

    public string Model
    {
        get => model;
        set
        {
            model = value;
            OnPropertyChanged(nameof(Model));
        }
    }

    public string SerialNumber
    {
        get => serialNumber;
        set
        {
            serialNumber = value;
            OnPropertyChanged(nameof(SerialNumber));
        }
    }

    public string Status
    {
        get => status;
        set
        {
            status = value;
            OnPropertyChanged(nameof(Status));
        }
    }

    public DateTime PurchaseDate { get; set; } = DateTime.Today;

    public string AssignedUser { get; set; } = "";

    public string Location
    {
        get => location;
        set
        {
            location = value;
            OnPropertyChanged(nameof(Location));
        }
    }

    public string Department
    {
        get => department;
        set
        {
            department = value;
            OnPropertyChanged(nameof(Department));
        }
    }

    public string Notes { get; set; } = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(name));
    }
}