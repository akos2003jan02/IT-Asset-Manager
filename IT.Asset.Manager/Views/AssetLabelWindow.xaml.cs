using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using IT.Asset.Manager.Services;

namespace IT.Asset.Manager.Views;

public partial class AssetLabelWindow : Window
{
    private readonly global::IT.Asset.Manager.Models.Asset asset;

    public AssetLabelWindow(
        global::IT.Asset.Manager.Models.Asset asset)
    {
        InitializeComponent();

        this.asset = asset;

        LoadAssetData();
        GenerateQrCode();
    }

    private void LoadAssetData()
    {
        InventoryIdText.Text =
            asset.InventoryId;

        DeviceText.Text =
            $"{asset.Manufacturer} {asset.Model}";

        SerialText.Text =
            $"S/N: {DisplayValue(asset.SerialNumber)}";

        DepartmentText.Text =
            $"Department: {DisplayValue(asset.Department)}";

        LocationText.Text =
            $"Location: {DisplayValue(asset.Location)}";
    }

    private void GenerateQrCode()
    {
        var service =
            new QrCodeService();

        using var bitmap =
            service.Generate(
                asset.InventoryId,
                8);

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

        image.StreamSource =
            stream;

        image.EndInit();

        image.Freeze();

        QrImage.Source = image;
    }

    private void Print_Click(
        object sender,
        RoutedEventArgs e)
    {
        var printDialog =
            new System.Windows.Controls.PrintDialog();

        if (printDialog.ShowDialog() != true)
        {
            return;
        }

        printDialog.PrintVisual(
            CreatePrintLabel(),
            $"Asset Label - {asset.InventoryId}");

        MessageBox.Show(
            "A címke elküldve a nyomtatóra.",
            "Print",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private System.Windows.Controls.Border CreatePrintLabel()
    {
        var label =
            new System.Windows.Controls.Border
            {
                Width = 265,
                Height = 151,
                Background =
                    System.Windows.Media.Brushes.White,
                BorderBrush =
                    System.Windows.Media.Brushes.Black,
                BorderThickness =
                    new Thickness(1),
                Padding =
                    new Thickness(8)
            };

        var grid =
            new System.Windows.Controls.Grid();

        grid.ColumnDefinitions.Add(
            new System.Windows.Controls.ColumnDefinition
            {
                Width = new GridLength(105)
            });

        grid.ColumnDefinitions.Add(
            new System.Windows.Controls.ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

        grid.RowDefinitions.Add(
            new System.Windows.Controls.RowDefinition
            {
                Height = GridLength.Auto
            });

        grid.RowDefinitions.Add(
            new System.Windows.Controls.RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            });

        grid.RowDefinitions.Add(
            new System.Windows.Controls.RowDefinition
            {
                Height = GridLength.Auto
            });

        var qrService =
            new QrCodeService();

        using var bitmap =
            qrService.Generate(
                asset.InventoryId,
                8);

        using var stream =
            new MemoryStream();

        bitmap.Save(
            stream,
            ImageFormat.Png);

        stream.Position = 0;

        var qrImage =
            new BitmapImage();

        qrImage.BeginInit();
        qrImage.CacheOption =
            BitmapCacheOption.OnLoad;
        qrImage.StreamSource =
            stream;
        qrImage.EndInit();
        qrImage.Freeze();

        var image =
            new System.Windows.Controls.Image
            {
                Source = qrImage,
                Width = 95,
                Height = 95,
                Stretch =
                    System.Windows.Media.Stretch.Uniform,
                HorizontalAlignment =
                    HorizontalAlignment.Center,
                VerticalAlignment =
                    VerticalAlignment.Center
            };

        System.Windows.Controls.Grid.SetRow(
            image,
            0);

        System.Windows.Controls.Grid.SetRowSpan(
            image,
            3);

        System.Windows.Controls.Grid.SetColumn(
            image,
            0);

        grid.Children.Add(image);

        var inventory =
            new System.Windows.Controls.TextBlock
            {
                Text = asset.InventoryId,
                Foreground =
                    System.Windows.Media.Brushes.Black,
                FontSize = 15,
                FontWeight =
                    FontWeights.Bold,
                TextWrapping =
                    TextWrapping.Wrap,
                Margin =
                    new Thickness(5, 2, 0, 3)
            };

        System.Windows.Controls.Grid.SetRow(
            inventory,
            0);

        System.Windows.Controls.Grid.SetColumn(
            inventory,
            1);

        grid.Children.Add(inventory);

        var device =
            new System.Windows.Controls.TextBlock
            {
                Text =
                    $"{asset.Manufacturer} {asset.Model}",
                Foreground =
                    System.Windows.Media.Brushes.Black,
                FontSize = 10,
                FontWeight =
                    FontWeights.SemiBold,
                TextWrapping =
                    TextWrapping.Wrap,
                VerticalAlignment =
                    VerticalAlignment.Center,
                Margin =
                    new Thickness(5, 0, 0, 0)
            };

        System.Windows.Controls.Grid.SetRow(
            device,
            1);

        System.Windows.Controls.Grid.SetColumn(
            device,
            1);

        grid.Children.Add(device);

        var details =
            new System.Windows.Controls.StackPanel
            {
                Margin =
                    new Thickness(5, 3, 0, 0)
            };

        details.Children.Add(
            CreateDetailText(
                $"S/N: {DisplayValue(asset.SerialNumber)}"));

        details.Children.Add(
            CreateDetailText(
                $"Department: {DisplayValue(asset.Department)}"));

        details.Children.Add(
            CreateDetailText(
                $"Location: {DisplayValue(asset.Location)}"));

        System.Windows.Controls.Grid.SetRow(
            details,
            2);

        System.Windows.Controls.Grid.SetColumn(
            details,
            1);

        grid.Children.Add(details);

        label.Child = grid;

        return label;
    }

    private static System.Windows.Controls.TextBlock CreateDetailText(
        string text)
    {
        return new System.Windows.Controls.TextBlock
        {
            Text = text,
            Foreground =
                System.Windows.Media.Brushes.Black,
            FontSize = 8,
            TextWrapping =
                TextWrapping.Wrap
        };
    }

    private static string DisplayValue(
        string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "-"
            : value;
    }

    private void Close_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
}