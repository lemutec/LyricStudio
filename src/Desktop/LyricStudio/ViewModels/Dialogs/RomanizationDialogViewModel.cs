using CommunityToolkit.Mvvm.ComponentModel;
using Fischless.Design.Controls;
using Fischless.Romanization;

namespace LyricStudio.ViewModels;

public partial class RomanizationDialogViewModel : ContentDialogViewModel
{
    [ObservableProperty]
    private int modeIndex = default;

    [ObservableProperty]
    private int lineTypeIndex = default;
}

public class RomanizationDialogSettings : IRomanizationDialogSettings
{
    public RomanizationMode Mode { get; set; }

    public RomanizationLineType LineType { get; set; }
}

public interface IRomanizationDialogSettings
{
    public RomanizationMode Mode { get; set; }

    public RomanizationLineType LineType { get; set; }
}

public enum RomanizationLineType
{
    Append,
    Replace,
}
