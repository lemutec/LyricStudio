using Avalonia.Controls;
using Avalonia.Input;
using LyricStudio.ViewModels;

namespace LyricStudio.Views;

public partial class HomePage : UserControl
{
    public HomePageViewModel ViewModel { get; }

    public HomePage() : this(App.GetService<HomePageViewModel>())
    {
    }

    public HomePage(HomePageViewModel viewModel)
    {
        DataContext = ViewModel = viewModel;
        InitializeComponent();
    }

    private void OnTimeClickBarTapped(object? sender, TappedEventArgs e)
    {
    }
}
