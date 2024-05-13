using FluentAvalonia.UI.Controls;

namespace Fischless.Design.Controls;

public interface IContentDialogViewModel
{
    public event Action<ContentDialogResult> CloseRequested;

    public ContentDialogResult Result { get; set; }
}
