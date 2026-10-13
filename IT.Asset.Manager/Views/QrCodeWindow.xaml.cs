using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using IT.Asset.Manager.Services;
using Microsoft.Win32;

namespace IT.Asset.Manager.Views;

public partial class QrCodeWindow : Window
{
    private readonly global::IT.Asset.Manager.Models.Asset asset;

    public QrCodeWindow(
        global::IT.Asset.Manager.Models.Asset asset)
    {
        InitializeComponent();

        this.asset = asset;

        InventoryIdText.Text =
            asset.InventoryId;

        DeviceText.Text =
            $"{asset.Manufacturer} {asset.Model}";

        SerialText.Text =
            $"Serial: {asset.SerialNumber}";

        GenerateQrCode();
    }

    private void GenerateQrCode()
    {
        var service =
            new QrCodeService();

        using var bitmap =
            service.Generate(
                asset.InventoryId);

        using var stream =
            new MemoryStream();

        bitmap.Save(
            stream,
            ImageFormat.Png);

        stream.Position = 0;

        var image =
            new BitmapImage();

        image.BeginInit();
        image.CacheOption =
            BitmapCacheOption.OnLoad;

        image.StreamSource = stream;

        image.EndInit();

        image.Freeze();

        QrImage.Source = image;
    }

    private void Save_Click(
        object sender,
        RoutedEventArgs e)
    {
        SaveFileDialog dialog = new()
        {
            Filter = "PNG Image (*.png)|*.png",
            FileName =
                $"{asset.InventoryId}_QR.png"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        var service =
            new QrCodeService();

        using var bitmap =
            service.Generate(
                asset.InventoryId);

        bitmap.Save(
            dialog.FileName,
            ImageFormat.Png);

        MessageBox.Show(
            "A QR-kód sikeresen elmentve.",
            "QR Code",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void Close_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
    private void PrintLabel_Click(
    object sender,
    RoutedEventArgs e)
    {
        var window =
            new AssetLabelWindow(asset);

        window.Owner = this;

        window.ShowDialog();
    }
}