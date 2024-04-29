using CommunityToolkit.Mvvm.Messaging;
using Fischless.Design.Controls;
using LyricStudio.Models.Messages;
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

        WeakReferenceMessenger.Default.Register<GlobalMessage>(this, (sender, msg) =>
        {
            if (msg.Command == nameof(GlobalCommand.ChangeMainWindowTitle))
            {
                if (string.IsNullOrWhiteSpace(msg.Param?.ToString()))
                {
                    Title = AppConfig.PackName;
                }
                else
                {
                    Title = $"{msg.Param} - {AppConfig.PackName}";
                }
            }
        });
    }
}
