using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using IT.Asset.Manager.Commands;
using IT.Asset.Manager.Data;
using IT.Asset.Manager.Services;

namespace IT.Asset.Manager.ViewModels;

public class AuditLogViewModel : INotifyPropertyChanged
{
    private List<global::IT.Asset.Manager.Models.AssetHistory> allHistory
        = new();

    private string searchText = "";

    private string selectedAction = "All";

    public ObservableCollection<global::IT.Asset.Manager.Models.AssetHistory> History
    {
        get;
    } = new();

    public ObservableCollection<string> Actions
    {
        get;
    } = new();

    public ICommand RestoreCommand { get; }

    public event Action? RestoreCompleted;

    public string SearchText
    {
        get => searchText;

        set
        {
            searchText = value;

            OnPropertyChanged();

            FilterHistory();
        }
    }

    public string SelectedAction
    {
        get => selectedAction;

        set
        {
            selectedAction = value;

            OnPropertyChanged();

            FilterHistory();
        }
    }

    public AuditLogViewModel()
    {
        RestoreCommand =
            new RelayCommand(
                history => Restore(history),
                history =>
                    history is
                    global::IT.Asset.Manager.Models.AssetHistory &&
                    ((global::IT.Asset.Manager.Models.AssetHistory)history)
                    .Action == "Deleted");

        LoadHistory();
    }

    private void LoadHistory()
    {
        using var db = new AppDbContext();

        allHistory = db.AssetHistories
            .OrderByDescending(x => x.Timestamp)
            .ToList();

        Actions.Clear();

        Actions.Add("All");

        foreach (var action in allHistory
            .Select(x => x.Action)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .OrderBy(x => x))
        {
            Actions.Add(action);
        }

        FilterHistory();
    }

    private void FilterHistory()
    {
        IEnumerable<global::IT.Asset.Manager.Models.AssetHistory> filtered =
            allHistory;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(x =>
                x.Manufacturer.Contains(
                    SearchText,
                    StringComparison.OrdinalIgnoreCase) ||

                x.Model.Contains(
                    SearchText,
                    StringComparison.OrdinalIgnoreCase) ||

                x.SerialNumber.Contains(
                    SearchText,
                    StringComparison.OrdinalIgnoreCase) ||

                x.Details.Contains(
                    SearchText,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedAction != "All")
        {
            filtered = filtered.Where(x =>
                x.Action == SelectedAction);
        }

        filtered = filtered
            .OrderByDescending(x => x.Timestamp);

        History.Clear();

        foreach (var item in filtered)
        {
            History.Add(item);
        }
    }

    private void Restore(object? parameter)
    {
        if (parameter is not
            global::IT.Asset.Manager.Models.AssetHistory history)
        {
            return;
        }

        var service = new AssetService();

        bool restored = service.Restore(history);

        if (!restored)
        {
            System.Windows.MessageBox.Show(
                "Az eszköz nem állítható vissza, mert ugyanazzal a Serial Numberrel már létezik egy eszköz.",
                "Restore failed",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);

            return;
        }

        LoadHistory();

        RestoreCompleted?.Invoke();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}