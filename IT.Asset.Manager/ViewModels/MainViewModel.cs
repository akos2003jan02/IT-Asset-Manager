using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using IT.Asset.Manager.Commands;
using IT.Asset.Manager.Data;
using IT.Asset.Manager.Services;
namespace IT.Asset.Manager.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<global::IT.Asset.Manager.Models.Asset> Assets
    {
        get;
    } = new();

    public ObservableCollection<global::IT.Asset.Manager.Models.ManufacturerStatistic> ManufacturerStats
    {
        get;
    } = new();

    public ObservableCollection<global::IT.Asset.Manager.Models.StatusStatistic> StatusStats
    {
        get;
    } = new();

    public ObservableCollection<global::IT.Asset.Manager.Models.AssetWarning> Warnings
    {
        get;
    } = new();

    public ObservableCollection<string> StatusFilters
    {
        get;
    } = new()
    {
        "All",
        "In Stock",
        "Issued",
        "Service"
    };

    public ObservableCollection<string> AssignmentFilters
    {
        get;
    } = new()
    {
        "All",
        "Assigned",
        "Unassigned"
    };

    public ICommand AddDeviceCommand { get; }
    public ICommand EditDeviceCommand { get; }
    public ICommand DeleteDeviceCommand { get; }
    public ICommand ClearFiltersCommand { get; }

    private List<global::IT.Asset.Manager.Models.Asset> allAssets = new();

    private string searchText = "";
    private string selectedStatus = "All";
    private string selectedAssignment = "All";

    public string SearchText
    {
        get => searchText;
        set
        {
            searchText = value;
            OnPropertyChanged();
            FilterAssets();
        }
    }

    public string SelectedStatus
    {
        get => selectedStatus;
        set
        {
            selectedStatus = value;
            OnPropertyChanged();
            FilterAssets();
        }
    }

    public string SelectedAssignment
    {
        get => selectedAssignment;
        set
        {
            selectedAssignment = value;
            OnPropertyChanged();
            FilterAssets();
        }
    }

    public int TotalDevices => Assets.Count;

    public int InStock =>
        Assets.Count(x => x.Status == "In Stock");

    public int Issued =>
        Assets.Count(x => x.Status == "Issued");

    public int Service =>
        Assets.Count(x => x.Status == "Service");

    public int WarningCount =>
        Warnings
            .Select(x => x.AssetId)
            .Distinct()
            .Count();

    public int TotalWarnings =>
        Warnings.Count;

    public MainViewModel()
    {
        AddDeviceCommand =
            new RelayCommand(_ => AddDevice());

        EditDeviceCommand =
            new RelayCommand(
                asset => EditDevice(asset),
                asset =>
                    asset is global::IT.Asset.Manager.Models.Asset);

        DeleteDeviceCommand =
            new RelayCommand(
                asset => DeleteDevice(asset),
                asset =>
                    asset is global::IT.Asset.Manager.Models.Asset);

        ClearFiltersCommand =
            new RelayCommand(_ => ClearFilters());

        LoadAssets();
    }

    public void LoadAssets()
    {
        using var db = new AppDbContext();

        allAssets = db.Assets
            .AsNoTracking()
            .ToList();

        FilterAssets();
        CheckWarnings();
    }

    private void FilterAssets()
    {
        using var db = new AppDbContext();

        IQueryable<global::IT.Asset.Manager.Models.Asset> query =
            db.Assets.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            string search = SearchText.Trim();

            query = query.Where(x =>
                x.InventoryId.Contains(search) ||
                x.Manufacturer.Contains(search) ||
                x.Model.Contains(search) ||
                x.SerialNumber.Contains(search) ||
                x.Location.Contains(search) ||
                x.Department.Contains(search) ||
                x.AssignedUser.Contains(search) ||
                x.Status.Contains(search));
        }

        if (SelectedStatus != "All")
        {
            query = query.Where(
                x => x.Status == SelectedStatus);
        }

        if (SelectedAssignment == "Assigned")
        {
            query = query.Where(
                x => !string.IsNullOrWhiteSpace(x.AssignedUser));
        }
        else if (SelectedAssignment == "Unassigned")
        {
            query = query.Where(
                x => string.IsNullOrWhiteSpace(x.AssignedUser));
        }

        var filtered = query.ToList();

        Assets.Clear();

        foreach (var item in filtered)
        {
            Assets.Add(item);
        }

        RefreshStats();
    }

    private void ClearFilters()
    {
        SearchText = "";
        SelectedStatus = "All";
        SelectedAssignment = "All";
    }

    private void RefreshStats()
    {
        OnPropertyChanged(nameof(TotalDevices));
        OnPropertyChanged(nameof(InStock));
        OnPropertyChanged(nameof(Issued));
        OnPropertyChanged(nameof(Service));

        ManufacturerStats.Clear();
        StatusStats.Clear();

        var manufacturerGroups =
            Assets
                .GroupBy(x => x.Manufacturer)
                .Select(x => new
                {
                    Manufacturer = x.Key,
                    Count = x.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

        int maxManufacturerCount =
            manufacturerGroups.Count > 0
                ? manufacturerGroups.Max(x => x.Count)
                : 1;

        foreach (var item in manufacturerGroups)
        {
            ManufacturerStats.Add(
                new global::IT.Asset.Manager.Models.ManufacturerStatistic
                {
                    Manufacturer = item.Manufacturer,
                    Count = item.Count,
                    MaxCount = maxManufacturerCount
                });
        }

        var statusGroups =
            Assets
                .GroupBy(x => x.Status)
                .Select(x => new
                {
                    Status = x.Key,
                    Count = x.Count()
                })
                .ToList();

        int maxStatusCount =
            statusGroups.Count > 0
                ? statusGroups.Max(x => x.Count)
                : 1;

        foreach (var item in statusGroups)
        {
            StatusStats.Add(
                new global::IT.Asset.Manager.Models.StatusStatistic
                {
                    Status = item.Status,
                    Count = item.Count,
                    MaxCount = maxStatusCount
                });
        }
    }

    private void CheckWarnings()
    {
        using var db = new AppDbContext();

        DateTime fiveYearsAgo =
            DateTime.Today.AddYears(-5);

        var warningAssets =
            db.Assets
                .AsNoTracking()
                .Where(asset =>
                    asset.Status == "Service" ||
                    asset.PurchaseDate < fiveYearsAgo ||
                    string.IsNullOrWhiteSpace(asset.AssignedUser) ||
                    string.IsNullOrWhiteSpace(asset.Manufacturer) ||
                    string.IsNullOrWhiteSpace(asset.Model) ||
                    string.IsNullOrWhiteSpace(asset.SerialNumber))
                .ToList();

        Warnings.Clear();

        foreach (var asset in warningAssets)
        {
            if (asset.Status == "Service")
            {
                Warnings.Add(
                    new global::IT.Asset.Manager.Models.AssetWarning
                    {
                        AssetId = asset.Id,
                        WarningType = "Service",
                        Message =
                            $"{asset.Manufacturer} {asset.Model} " +
                            "szervizben van.",
                        Severity = "Critical",
                        ActionType = "Edit"
                    });
            }

            if (asset.PurchaseDate < fiveYearsAgo)
            {
                Warnings.Add(
                    new global::IT.Asset.Manager.Models.AssetWarning
                    {
                        AssetId = asset.Id,
                        WarningType = "Old Asset",
                        Message =
                            $"{asset.Manufacturer} {asset.Model} " +
                            "több mint 5 éves.",
                        Severity = "Warning",
                        ActionType = "Edit"
                    });
            }

            if (string.IsNullOrWhiteSpace(asset.AssignedUser))
            {
                Warnings.Add(
                    new global::IT.Asset.Manager.Models.AssetWarning
                    {
                        AssetId = asset.Id,
                        WarningType = "Unassigned",
                        Message =
                            $"{asset.Manufacturer} {asset.Model} " +
                            "nincs felhasználóhoz rendelve.",
                        Severity = "Warning",
                        ActionType = "AssignUser"
                    });
            }

            if (string.IsNullOrWhiteSpace(asset.Manufacturer) ||
                string.IsNullOrWhiteSpace(asset.Model) ||
                string.IsNullOrWhiteSpace(asset.SerialNumber))
            {
                Warnings.Add(
                    new global::IT.Asset.Manager.Models.AssetWarning
                    {
                        AssetId = asset.Id,
                        WarningType = "Incomplete Data",
                        Message =
                            $"{asset.Manufacturer} {asset.Model} " +
                            "adatai hiányosak.",
                        Severity = "Critical",
                        ActionType = "Edit"
                    });
            }
        }

        OnPropertyChanged(nameof(WarningCount));
        OnPropertyChanged(nameof(TotalWarnings));
    }

    private void AddDevice()
    {
        AddRequested?.Invoke();
    }

    public event Action? AddRequested;

    public event Action<
        global::IT.Asset.Manager.Models.Asset>? EditRequested;

    private void EditDevice(object? parameter)
    {
        if (parameter is not
            global::IT.Asset.Manager.Models.Asset asset)
        {
            return;
        }

        EditRequested?.Invoke(asset);
    }

    private void DeleteDevice(object? parameter)
    {
        if (parameter is not
            global::IT.Asset.Manager.Models.Asset asset)
        {
            return;
        }

        var service = new AssetService();

        service.Delete(asset.Id);

        LoadAssets();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(name));
    }
}