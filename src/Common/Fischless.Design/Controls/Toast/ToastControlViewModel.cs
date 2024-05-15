using CommunityToolkit.Mvvm.ComponentModel;

namespace Fischless.Design.Controls;

public partial class ToastControlViewModel : ObservableObject
{
    [ObservableProperty]
    public string message = null!;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasIcon))]
    public string imageGlyph = null!;

    public bool HasIcon => ImageGlyph is not null;
}
