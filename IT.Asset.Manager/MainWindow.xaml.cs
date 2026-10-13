using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IT.Asset.Manager.Data;
using IT.Asset.Manager.Services;
using IT.Asset.Manager.ViewModels;
using IT.Asset.Manager.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace IT.Asset.Manager;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var viewModel = new MainViewModel();

        viewModel.AddRequested += AddAsset;
        viewModel.EditRequested += EditAsset;

        DataContext = viewModel;

        PreviewKeyDown += MainWindow_PreviewKeyDown;
    }

    private void MainWindow_PreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key == Key.F &&
            Keyboard.Modifiers == ModifierKeys.Control)
        {
            SearchBox.Focus();
            SearchBox.SelectAll();

            e.Handled = true;
            return;
        }

        if (e.Key == Key.N &&
            Keyboard.Modifiers == ModifierKeys.Control)
        {
            AddAsset();

            e.Handled = true;
            return;
        }

        if (e.Key == Key.C &&
            Keyboard.Modifiers ==
            (ModifierKeys.Control | ModifierKeys.Shift))
        {
            CopySelectedSerial();

            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape)
        {
            Close();

            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            if (Keyboard.FocusedElement is TextBox)
            {
                return;
            }

            OpenSelectedAsset();

            e.Handled = true;
            return;
        }

        if (e.Key == Key.Delete)
        {
            if (Keyboard.FocusedElement is TextBox)
            {
                return;
            }

            DeleteSelectedAsset();

            e.Handled = true;
        }
    }

    private void OpenSelectedAsset()
    {
        if (AssetsGrid.SelectedItem is not
            global::IT.Asset.Manager.Models.Asset asset)
        {
            return;
        }

        var details = new AssetDetailsWindow(asset);

        details.Owner = this;

        if (details.ShowDialog() == true)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.LoadAssets();
            }
        }
    }

    private void DeleteSelectedAsset()
    {
        var selectedAssets =
            AssetsGrid.SelectedItems
                .Cast<global::IT.Asset.Manager.Models.Asset>()
                .ToList();

        if (selectedAssets.Count == 0)
        {
            return;
        }

        string message;

        if (selectedAssets.Count == 1)
        {
            var asset = selectedAssets[0];

            message =
                $"Biztosan törölni szeretnéd ezt az eszközt?\n\n" +
                $"{asset.Manufacturer} {asset.Model}\n" +
                $"Serial: {asset.SerialNumber}";
        }
        else
        {
            message =
                $"Biztosan törölni szeretnéd a(z) " +
                $"{selectedAssets.Count} kijelölt eszközt?";
        }

        var result = MessageBox.Show(
            message,
            "Delete Devices",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        using var db = new AppDbContext();

        foreach (var selectedAsset in selectedAssets)
        {
            var asset = db.Assets
                .FirstOrDefault(x => x.Id == selectedAsset.Id);

            if (asset == null)
            {
                continue;
            }

            db.AssetHistories.Add(
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

            db.Assets.Remove(asset);
        }

        db.SaveChanges();

        if (DataContext is MainViewModel viewModel)
        {
            viewModel.LoadAssets();
        }
    }

    private void CopySelectedSerial()
    {
        if (AssetsGrid.SelectedItem is not
            global::IT.Asset.Manager.Models.Asset asset)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(asset.SerialNumber))
        {
            return;
        }

        Clipboard.SetText(asset.SerialNumber);

        MessageBox.Show(
            $"Serial Number kimásolva:\n\n{asset.SerialNumber}",
            "Copied",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void ChangeSelectedStatus()
    {
        var selectedAssets =
            AssetsGrid.SelectedItems
                .Cast<global::IT.Asset.Manager.Models.Asset>()
                .ToList();

        if (selectedAssets.Count == 0)
        {
            return;
        }

        var window = new BulkStatusWindow();

        window.Owner = this;

        if (window.ShowDialog() != true)
        {
            return;
        }

        using var db = new AppDbContext();

        foreach (var selectedAsset in selectedAssets)
        {
            var asset = db.Assets
                .FirstOrDefault(x => x.Id == selectedAsset.Id);

            if (asset == null)
            {
                continue;
            }

            string oldStatus = asset.Status;

            if (oldStatus == window.SelectedStatus)
            {
                continue;
            }

            asset.Status = window.SelectedStatus;

            db.AssetHistories.Add(
                new global::IT.Asset.Manager.Models.AssetHistory
                {
                    AssetId = asset.Id,
                    Manufacturer = asset.Manufacturer,
                    Model = asset.Model,
                    SerialNumber = asset.SerialNumber,
                    Action = "Status changed",
                    Details =
                        $"{oldStatus} → {window.SelectedStatus}",
                    Timestamp = DateTime.Now
                });
        }

        db.SaveChanges();

        if (DataContext is MainViewModel viewModel)
        {
            viewModel.LoadAssets();
        }
    }

    private void AssignSelectedUser()
    {
        var selectedAssets =
            AssetsGrid.SelectedItems
                .Cast<global::IT.Asset.Manager.Models.Asset>()
                .ToList();

        if (selectedAssets.Count == 0)
        {
            return;
        }

        var window = new BulkAssignWindow();

        window.Owner = this;

        if (window.ShowDialog() != true)
        {
            return;
        }

        using var db = new AppDbContext();

        foreach (var selectedAsset in selectedAssets)
        {
            var asset = db.Assets
                .FirstOrDefault(x => x.Id == selectedAsset.Id);

            if (asset == null)
            {
                continue;
            }

            string oldUser = asset.AssignedUser;

            if (oldUser == window.AssignedUser)
            {
                continue;
            }

            asset.AssignedUser = window.AssignedUser;

            db.AssetHistories.Add(
                new global::IT.Asset.Manager.Models.AssetHistory
                {
                    AssetId = asset.Id,
                    Manufacturer = asset.Manufacturer,
                    Model = asset.Model,
                    SerialNumber = asset.SerialNumber,
                    Action = "Assigned User changed",
                    Details =
                        $"{DisplayValue(oldUser)} → " +
                        $"{DisplayValue(window.AssignedUser)}",
                    Timestamp = DateTime.Now
                });
        }

        db.SaveChanges();

        if (DataContext is MainViewModel viewModel)
        {
            viewModel.LoadAssets();
        }
    }

    private void AddAsset()
    {
        var window = new AddAssetWindow();

        window.Owner = this;

        if (window.ShowDialog() == true)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.LoadAssets();
            }
        }
    }

    private void EditAsset(
        global::IT.Asset.Manager.Models.Asset asset)
    {
        var window = new AddAssetWindow(asset);

        window.Owner = this;

        if (window.ShowDialog() == true)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.LoadAssets();
            }
        }
    }

    private void ImportExcel_Click(
    object sender,
    RoutedEventArgs e)
    {
        OpenFileDialog dialog = new();

        dialog.Filter =
            "Excel (*.xlsx)|*.xlsx";

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        var previewWindow =
            new ImportPreviewWindow(dialog.FileName);

        previewWindow.Owner = this;

        if (previewWindow.ShowDialog() == true)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.LoadAssets();
            }
        }
    }

    private void ExportExcel_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        if (viewModel.Assets.Count == 0)
        {
            MessageBox.Show(
                "Nincs exportálható eszköz.",
                "Export",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        SaveFileDialog dialog = new()
        {
            Filter = "Excel (*.xlsx)|*.xlsx",
            FileName = $"IT_Assets_{DateTime.Today:yyyy-MM-dd}.xlsx"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            var service = new ExportService();

            service.Export(
                dialog.FileName,
                viewModel.Assets);

            MessageBox.Show(
                $"{viewModel.Assets.Count} eszköz sikeresen exportálva.\n\n" +
                $"Fájl:\n{dialog.FileName}",
                "Export completed",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Az exportálás sikertelen.\n\n{ex.Message}",
                "Export error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void AssetsGrid_MouseDoubleClick(
        object sender,
        MouseButtonEventArgs e)
    {
        OpenSelectedAsset();
    }

    private void DeleteDevice_Click(
        object sender,
        RoutedEventArgs e)
    {
        DeleteSelectedAsset();
    }

    private void ChangeStatus_Click(
        object sender,
        RoutedEventArgs e)
    {
        ChangeSelectedStatus();
    }

    private void AssignUser_Click(
        object sender,
        RoutedEventArgs e)
    {
        AssignSelectedUser();
    }

    private void AuditLog_Click(
    object sender,
    RoutedEventArgs e)
    {
        var window = new AuditLogWindow();

        window.Owner = this;

        window.RestoreCompleted += () =>
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.LoadAssets();
            }
        };

        window.ShowDialog();
    }

    private static string DisplayValue(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "(empty)"
            : value;
    }
    private void Warning_MouseLeftButtonDown(
    object sender,
    MouseButtonEventArgs e)
    {
        if (e.ClickCount != 2)
        {
            return;
        }

        if (sender is not Border border)
        {
            return;
        }

        if (border.DataContext is not
            global::IT.Asset.Manager.Models.AssetWarning warning)
        {
            return;
        }

        using var db = new AppDbContext();

        var asset = db.Assets.FirstOrDefault(
            x => x.Id == warning.AssetId);

        if (asset == null)
        {
            return;
        }

        if (warning.ActionType == "AssignUser")
        {
            var window = new BulkAssignWindow();

            window.Owner = this;

            if (window.ShowDialog() == true)
            {
                asset.AssignedUser = window.AssignedUser;

                db.SaveChanges();

                db.AssetHistories.Add(
                    new global::IT.Asset.Manager.Models.AssetHistory
                    {
                        AssetId = asset.Id,
                        Manufacturer = asset.Manufacturer,
                        Model = asset.Model,
                        SerialNumber = asset.SerialNumber,
                        Action = "Assigned User changed",
                        Details =
                            $"(empty) → {DisplayValue(asset.AssignedUser)}",
                        Timestamp = DateTime.Now
                    });

                db.SaveChanges();

                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.LoadAssets();
                }
            }

            return;
        }

        if (warning.ActionType == "Edit")
        {
            var window = new AddAssetWindow(asset);

            window.Owner = this;

            if (window.ShowDialog() == true)
            {
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.LoadAssets();
                }
            }
        }
    }
    

    
}