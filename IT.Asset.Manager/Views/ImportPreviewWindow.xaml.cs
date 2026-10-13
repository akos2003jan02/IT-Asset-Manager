using System.Windows;
using IT.Asset.Manager.Models;
using IT.Asset.Manager.Services;

namespace IT.Asset.Manager.Views;

public partial class ImportPreviewWindow : Window
{
    private readonly List<ImportPreviewItem> items;

    public int ImportedCount { get; private set; }

    public ImportPreviewWindow(string filePath)
    {
        InitializeComponent();

        var service = new ImportPreviewService();

        items = service.Preview(filePath);

        PreviewGrid.ItemsSource = items;

        UpdateSummary();
    }

    private void UpdateSummary()
    {
        int total = items.Count;

        int ready =
            items.Count(x => x.Result == "Ready");

        int duplicate =
            items.Count(x => x.Result == "Duplicate");

        int error =
            items.Count(x => x.Result == "Error");

        TotalText.Text = total.ToString();
        ReadyText.Text = ready.ToString();
        DuplicateText.Text = duplicate.ToString();
        ErrorText.Text = error.ToString();
    }

    private void Import_Click(object sender, RoutedEventArgs e)
    {
        var validItems =
            items
                .Where(x => x.IsValid)
                .ToList();

        if (validItems.Count == 0)
        {
            MessageBox.Show(
                "Nincs importálható eszköz.",
                "Import",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var result = MessageBox.Show(
            $"{validItems.Count} eszköz kerül importálásra.\n\n" +
            "A hibás és duplikált sorok kimaradnak.\n\n" +
            "Folytatod?",
            "Confirm Import",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        var service = new ImportPreviewService();

        ImportedCount = service.Import(validItems);

        MessageBox.Show(
            $"{ImportedCount} eszköz sikeresen importálva.",
            "Import completed",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}