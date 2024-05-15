using Avalonia.Controls;
using Fischless.Win32;

namespace Fischless.Design.Controls;

public partial class LoadingDialog : Window
{
    public LoadingDialogViewModel ViewModel { get; }
    public LoadingBoxResult Result => ViewModel.Result;

    public LoadingDialog()
    {
        DataContext = ViewModel = new();
        ViewModel.CloseRequested += OnCloseRequested;
        InitializeComponent();

        ApplyFluentStyling();
    }

    private void ApplyFluentStyling()
    {
        _ = WindowBackdrop.PrepareBackground(this, WindowBackdropType.Mica);
        _ = WindowBackdrop.ApplyBackdrop(this, WindowBackdropType.Mica);
        Activated += OnActivated;
    }

    protected virtual void OnCloseRequested(LoadingBoxResult e)
    {
        Close(e);
    }

    protected virtual void OnActivated(object? sender, EventArgs e)
    {
        Activated -= OnActivated;

        // Just case the window is activated, we will re-apply the backdrop
        _ = WindowBackdrop.ApplyBackdrop(this, WindowBackdropType.Mica);
    }

    public async Task ShowDialog(Window owner, CancellationToken? token)
    {
        if (token == null)
        {
            await ShowDialog(owner);
        }
        else
        {
            TaskCompletionSource<bool> tcs = new();

            token?.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: true);
            Closed += (sender, e) => tcs.TrySetResult(true);
            _ = ShowDialog(owner);
            await tcs.Task;
        }
    }
}
