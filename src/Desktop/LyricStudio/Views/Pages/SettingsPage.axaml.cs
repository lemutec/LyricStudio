using Avalonia.Controls;
using LyricStudio.ViewModels;

namespace LyricStudio.Views;

public partial class SettingsPage : UserControl
{
    public SettingsPageViewModel ViewModel { get; }

    public SettingsPage() : this(App.GetService<SettingsPageViewModel>())
    {
    }

    public SettingsPage(SettingsPageViewModel viewModel)
    {
        DataContext = ViewModel = viewModel;
        InitializeComponent();
    }
}
