using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Fischless.Design.Controls;

public static class LoadingBox
{
    public static async Task<LoadingBoxResult> ShowAsync(string messageBoxText = null, string caption = null, CancellationToken? token = null)
        => await ShowAsync(GetActiveWindow(), messageBoxText, caption, token);

    public static async Task<LoadingBoxResult> ShowAsync(Window? owner, string messageBoxText, string caption = null, CancellationToken? token = null)
    {
        owner ??= GetActiveWindow();

        // TODO: messageBoxText/caption
        LoadingDialog dialog = new()
        {
            Icon = owner?.Icon,
            Topmost = owner?.Topmost ?? false,
        };
        await dialog.ShowDialog(owner, token);

        return dialog.Result;
    }

    private static Window? GetActiveWindow()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.Windows.FirstOrDefault(window => window.IsActive && window.ShowActivated) ?? desktop.MainWindow;
        }

        return null;
    }
}
