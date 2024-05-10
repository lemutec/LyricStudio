using Fischless.Design.Controls;
using FluentAvalonia.UI.Controls;
using LyricStudio.ViewModels;
using System;
using System.Threading.Tasks;

namespace LyricStudio.Views;

public partial class ShiftTimecodeDialog : FluentWindow
{
    public ShiftTimecodeDialogViewModel ViewModel { get; }

    public ShiftTimecodeDialog() : this(App.GetService<ShiftTimecodeDialogViewModel>())
    {
    }

    public ShiftTimecodeDialog(ShiftTimecodeDialogViewModel viewModel)
    {
        DataContext = ViewModel = viewModel;
        InitializeComponent();

        ViewModel.RequestClose += RequestClose;
    }

    private void RequestClose(object? sender, ContentDialogResult e)
    {
        Close(e);
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
