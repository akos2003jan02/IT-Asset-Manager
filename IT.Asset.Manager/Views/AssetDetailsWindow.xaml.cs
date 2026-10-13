using System.Windows;
using IT.Asset.Manager.Data;
using IT.Asset.Manager.Models;

namespace IT.Asset.Manager.Views;

public partial class AssetDetailsWindow : Window
{
    private readonly global::IT.Asset.Manager.Models.Asset asset;

    public AssetDetailsWindow(
        global::IT.Asset.Manager.Models.Asset asset)
    {
        InitializeComponent();

        this.asset = asset;

        LoadAsset();
        LoadHistory();
    }

    private void LoadAsset()
    {
        InventoryIdText.Text =
            asset.InventoryId;

        InventoryIdValue.Text =
            asset.InventoryId;

        ManufacturerText.Text =
            DisplayValue(asset.Manufacturer);

        ModelText.Text =
            DisplayValue(asset.Model);

        SerialText.Text =
            DisplayValue(asset.SerialNumber);

        StatusText.Text =
            DisplayValue(asset.Status);

        AssignedUserText.Text =
            DisplayValue(asset.AssignedUser);

        LocationText.Text =
            DisplayValue(asset.Location);

        DepartmentText.Text =
            DisplayValue(asset.Department);

        PurchaseDateText.Text =
            asset.PurchaseDate.ToString("yyyy.MM.dd");

        NotesText.Text =
            DisplayValue(asset.Notes);
    }

    private void LoadHistory()
    {
        using var db = new AppDbContext();

        var history =
            db.AssetHistories
                .Where(x => x.AssetId == asset.Id)
                .OrderByDescending(x => x.Timestamp)
                .ToList();

        HistoryGrid.ItemsSource = history;
    }

    private void QrCode_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window =
            new QrCodeWindow(asset);

        window.Owner = this;

        window.ShowDialog();
    }

    private void Edit_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window =
            new AddAssetWindow(asset);

        window.Owner = this;

        if (window.ShowDialog() == true)
        {
            LoadAsset();
            LoadHistory();
        }
    }

    private void Close_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }

    private static string DisplayValue(
        string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "(empty)"
            : value;
    }
}