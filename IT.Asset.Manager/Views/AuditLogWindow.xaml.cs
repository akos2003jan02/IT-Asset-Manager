using System.Windows;
using IT.Asset.Manager.ViewModels;

namespace IT.Asset.Manager.Views;

public partial class AuditLogWindow : Window
{
    public event Action? RestoreCompleted;

    private readonly AuditLogViewModel viewModel;

    public AuditLogWindow()
    {
        InitializeComponent();

        viewModel = new AuditLogViewModel();

        viewModel.RestoreCompleted += OnRestoreCompleted;

        DataContext = viewModel;
    }

    private void OnRestoreCompleted()
    {
        RestoreCompleted?.Invoke();
    }

    protected override void OnClosed(
        EventArgs e)
    {
        viewModel.RestoreCompleted -= OnRestoreCompleted;

        base.OnClosed(e);
    }
}