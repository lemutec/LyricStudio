using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Messaging;
using Fischless.Win32.Helpers;
using LyricStudio.Models.Messages;
using LyricStudio.ViewModels;
using System.Linq;

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
        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.GetFileNames() is { } fileNames)
        {
            WeakReferenceMessenger.Default.Send(new FileDropMessage(fileNames.ToArray()));
        }
    }

    private void OnTimeClickBarTapped(object? sender, TappedEventArgs e)
    {
    }
}
