using Avalonia.Controls;
using Fischless.Win32;

namespace Fischless.Design.Controls;

public class FluentWindow : Window
{
    public FluentWindow()
    {
        _ = WindowBackdrop.PrepareBackground(this, WindowBackdropType.Mica);
        _ = WindowBackdrop.ApplyBackdrop(this, WindowBackdropType.Mica);
        Activated += OnActivated;
    }

    protected virtual void OnActivated(object? sender, EventArgs e)
    {
        Activated -= OnActivated;

        // Just case the window is activated, we will re-apply the backdrop
        _ = WindowBackdrop.ApplyBackdrop(this, WindowBackdropType.Mica);
    }
}
