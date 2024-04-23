using Avalonia.Input;
using CommunityToolkit.Mvvm.Messaging;
using Fischless.Design.Controls;
using Fischless.Win32.Helpers;
using LyricStudio.Models.Messages;
using LyricStudio.ViewModels;
using System.Linq;

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
        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.GetFileNames() is { } fileNames)
        {
            WeakReferenceMessenger.Default.Send(new FileDropMessage(fileNames.ToArray()));
        }
    }
}
