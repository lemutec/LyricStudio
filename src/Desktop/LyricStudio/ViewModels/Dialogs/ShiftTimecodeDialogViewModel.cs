using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using System;

namespace LyricStudio.ViewModels;

public partial class ShiftTimecodeDialogViewModel : ObservableObject
{
    public event EventHandler<ContentDialogResult> RequestClose = null!;

    [ObservableProperty]
    private ContentDialogResult result = ContentDialogResult.None;

    [ObservableProperty]
    private double shiftTime = default;

    [RelayCommand]
    private void Accept()
    {
        Result = ContentDialogResult.Primary;
        RequestClose?.Invoke(this, Result);
    }

    [RelayCommand]
    private void Cancel()
    {
        Result = ContentDialogResult.Secondary;
        RequestClose?.Invoke(this, Result);
    }
}
