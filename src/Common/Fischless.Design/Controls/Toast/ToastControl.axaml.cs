using Avalonia.Controls;

namespace Fischless.Design.Controls;

public partial class ToastControl : UserControl
{
    public ToastControlViewModel ViewModel { get; }

    public string Message
    {
        get => ViewModel.Message;
        set => ViewModel.Message = value;
    }

    public string ImageGlyph
    {
        get => ViewModel.ImageGlyph;
        set => ViewModel.ImageGlyph = value;
    }

    public ToastControl()
    {
        DataContext = ViewModel = new();
        InitializeComponent();
    }
}
