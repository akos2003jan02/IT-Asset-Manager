using System.Windows;
using System.Windows.Controls;

namespace IT.Asset.Manager.Views;

public partial class BulkStatusWindow : Window
{
    public string SelectedStatus { get; private set; }
        = "In Stock";

    public BulkStatusWindow()
    {
        InitializeComponent();
    }

    private void Save_Click(
        object sender,
        RoutedEventArgs e)
    {
        SelectedStatus =
            (StatusBox.SelectedItem as ComboBoxItem)
            ?.Content
            ?.ToString()
            ?? "In Stock";

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}