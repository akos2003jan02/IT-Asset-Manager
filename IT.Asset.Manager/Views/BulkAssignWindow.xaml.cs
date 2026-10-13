using System.Windows;

namespace IT.Asset.Manager.Views;

public partial class BulkAssignWindow : Window
{
    public string AssignedUser { get; private set; } = "";

    public BulkAssignWindow()
    {
        InitializeComponent();

        Loaded += (_, _) =>
        {
            UserBox.Focus();
        };
    }

    private void Save_Click(
        object sender,
        RoutedEventArgs e)
    {
        AssignedUser = UserBox.Text.Trim();

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