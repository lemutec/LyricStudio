using Fischless.Design.Controls;
using LyricStudio.ViewModels;

namespace LyricStudio.Views;

public partial class MainWindow : FluentWindow
{
    public MainWindowViewModel ViewModel { get; }

    public MainWindow() : this(App.GetService<MainWindowViewModel>())
    {
    }

    public MainWindow(MainWindowViewModel viewModel)
    {
        DataContext = ViewModel = viewModel;
        InitializeComponent();
    }
}
