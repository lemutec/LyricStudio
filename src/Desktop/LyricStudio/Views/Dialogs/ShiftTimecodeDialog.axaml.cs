using Fischless.Design.Controls;
using FluentAvalonia.UI.Controls;
using LyricStudio.ViewModels;
using System.Threading.Tasks;

namespace LyricStudio.Views;

public partial class ShiftTimecodeDialog : FluentContentDialog
{
    public new ShiftTimecodeDialogViewModel ViewModel
    {
        get => base.ViewModel as ShiftTimecodeDialogViewModel;
        protected set => base.ViewModel = value;
    }

    public ShiftTimecodeDialog() : this(App.GetService<ShiftTimecodeDialogViewModel>())
    {
    }

    public ShiftTimecodeDialog(ShiftTimecodeDialogViewModel viewModel)
    {
        DataContext = ViewModel = viewModel;
        InitializeComponent();
    }

    public async Task<double?> GetShiftTimeAsync()
    {
        await ShowDialog(App.GetService<MainWindow>());

        if (ViewModel.Result != ContentDialogResult.Primary)
        {
            return null!;
        }
        return ViewModel.ShiftTime;
    }
}
