using Avalonia.Controls;
using Fischless.Win32;

namespace Fischless.Design.Controls;

public class FluentWindow : Window
{
    private bool _firstRevealScheduled;

    public FluentWindow()
    {
        ApplyFluentStyling();

        // Cloak as early as HWND exists so the first Show/ShowDialog cannot flash an empty frame.
        _ = WindowCloak.ApplyCloak(this, cloaked: true);
    }

    public override void Show()
    {
        _ = WindowCloak.ApplyCloak(this, cloaked: true);
        base.Show();
        ScheduleFirstReveal();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        // ShowDialog goes through ShowCore, not Show(); Opened still covers that path.
        ScheduleFirstReveal();
    }

    private void ScheduleFirstReveal()
    {
        if (_firstRevealScheduled)
        {
            return;
        }

        _firstRevealScheduled = true;
        WindowCloak.ScheduleUncloak(this);
    }

    private void ApplyFluentStyling()
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
