using Fischless.Design.Controls;
using Fischless.Romanization;
using FluentAvalonia.UI.Controls;
using LyricStudio.ViewModels;
using System.Threading.Tasks;

namespace LyricStudio.Views;

public partial class RomanizationDialog : FluentContentDialog
{
    public new RomanizationDialogViewModel ViewModel
    {
        get => base.ViewModel as RomanizationDialogViewModel;
        protected set => base.ViewModel = value;
    }

    public RomanizationDialog() : this(App.GetService<RomanizationDialogViewModel>())
    {
    }

    public RomanizationDialog(RomanizationDialogViewModel viewModel)
    {
        DataContext = ViewModel = viewModel;
        InitializeComponent();
    }

    public async Task<IRomanizationDialogSettings?> GetRomanizationSettingsAsync()
    {
        await ShowDialog(App.GetService<MainWindow>());

        if (ViewModel.Result != ContentDialogResult.Primary)
        {
            return null!;
        }
        return new RomanizationDialogSettings()
        {
            Mode = (RomanizationMode)ViewModel.ModeIndex,
            LineType = (RomanizationLineType)ViewModel.LineTypeIndex,
        };
    }
}
