using Avalonia.Controls;
using LyricStudio.ViewModels;

namespace LyricStudio.Views;

public partial class PluginPage : UserControl
{
    public PluginPageViewModel ViewModel { get; }

    public PluginPage() : this(App.GetService<PluginPageViewModel>())
    {
    }

    public PluginPage(PluginPageViewModel viewModel)
    {
        DataContext = ViewModel = viewModel;
        InitializeComponent();
    }
}
