using Fischless.Design.Controls;
using LyricStudio.ViewModels;

namespace LyricStudio.Views;

public partial class ShareDialog : FluentWindow
{
    public string CodeToShare
    {
        get => ViewModel.CodeToShare;
        set => ViewModel.CodeToShare = value;
    }

    public ShareDialogViewModel ViewModel { get; }

    public ShareDialog() : this(App.GetService<ShareDialogViewModel>())
    {
    }

    public ShareDialog(ShareDialogViewModel viewModel)
    {
        DataContext = ViewModel = viewModel;
        InitializeComponent();
    }
}
