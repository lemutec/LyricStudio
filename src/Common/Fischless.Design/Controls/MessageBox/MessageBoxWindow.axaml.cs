using Avalonia.Controls;
using Fischless.Win32;

namespace Fischless.Design.Controls;

public partial class MessageBoxWindow : Window
{
    public MessageBoxStandardViewModel ViewModel { get; }

    public MessageBoxResult Result => ViewModel.Result;

    public MessageBoxWindow() : this(new())
    {
    }

    public MessageBoxWindow(MessageBoxStandardParams @params)
    {
        DataContext = ViewModel = new MessageBoxStandardViewModel(@params);
        ViewModel.CloseRequested += OnCloseRequested;

        InitializeComponent();
        ShowInTaskbar = false;
        CanResize = false;

        ApplyFluentStyling();
    }

    private void ApplyFluentStyling()
    {
        _ = WindowBackdrop.PrepareBackground(this, WindowBackdropType.Mica);
        _ = WindowBackdrop.ApplyBackdrop(this, WindowBackdropType.Mica);
        Activated += OnActivated;
    }

    protected virtual void OnCloseRequested(MessageBoxResult e)
    {
        Close(e);
    }

    protected virtual void OnActivated(object? sender, EventArgs e)
    {
        Activated -= OnActivated;

        // Just case the window is activated, we will re-apply the backdrop
        _ = WindowBackdrop.ApplyBackdrop(this, WindowBackdropType.Mica);
    }

    public Task Copy()
    {
        var clipboard = TopLevel.GetTopLevel(this).Clipboard;
        var text = ContentTextBox.SelectedText;
        if (string.IsNullOrEmpty(text))
        {
            text = ViewModel?.ContentMessage;
        }
        return clipboard?.SetTextAsync(text);
    }
}
