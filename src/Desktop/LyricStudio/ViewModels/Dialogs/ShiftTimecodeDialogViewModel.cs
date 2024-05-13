using CommunityToolkit.Mvvm.ComponentModel;
using Fischless.Design.Controls;

namespace LyricStudio.ViewModels;

public partial class ShiftTimecodeDialogViewModel : ContentDialogViewModel
{
    [ObservableProperty]
    private double shiftTime = default;
}
