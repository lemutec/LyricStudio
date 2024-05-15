using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Fischless.Design.Controls;

public partial class LoadingDialogViewModel : ObservableObject
{
    public event Action<LoadingBoxResult> CloseRequested;

    public LoadingBoxResult Result { get; set; }

    public LoadingBoxButtonText Text { get; }

    public LoadingDialogViewModel()
    {
        Text = new LoadingBoxButtonText();
    }

    public void Close()
    {
        CloseRequested?.Invoke(Result);
    }

    public void SetButtonResult(LoadingBoxResult result)
    {
        Result = result;
    }

    [RelayCommand]
    public void Cancel()
    {
        SetButtonResult(LoadingBoxResult.Cancel);
        Close();
    }
}
