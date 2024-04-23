using Avalonia.Controls;
using Fischless.Design.Controls;
using LyricStudio.Services;
using LyricStudio.ViewModels;

namespace LyricStudio.Views;

public partial class MainView : UserControl
{
    public MainViewViewModel ViewModel { get; }

    public MainView() : this(App.GetService<MainViewViewModel>())
    {
    }

    public MainView(MainViewViewModel viewModel)
    {
        DataContext = ViewModel = viewModel;
        InitializeComponent();
        ViewLocator.Services = App.Services;
        App.GetService<INavigationService>()?.SetFrame(FrameView);
        App.GetService<INavigationService>()?.SetNavigationView(NavigationView);
        App.GetService<INavigationService>()?.Navigate(typeof(HomePage));
    }
}
