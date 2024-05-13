using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;

namespace Fischless.Design.Controls;

public partial class ContentDialogViewModel : ObservableObject, IContentDialogViewModel
{
    public event Action<ContentDialogResult> CloseRequested = null!;

    public ContentDialogResult Result { get; set; } = ContentDialogResult.None;

    public void RequestClose(ContentDialogResult result)
    {
        Result = result;
        CloseRequested?.Invoke(result);
    }

    [RelayCommand]
    protected virtual void Accept()
    {
        Result = ContentDialogResult.Primary;
        RequestClose(Result);
    }

    [RelayCommand]
    protected virtual void Cancel()
    {
        Result = ContentDialogResult.Secondary;
        RequestClose(Result);
    }
}
