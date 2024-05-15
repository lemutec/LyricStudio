using Avalonia.Controls;
using Avalonia.Input;
using Fischless.Win32.Helpers;
using LyricStudio.ViewModels;
using System.Linq;

namespace LyricStudio.Views;

public partial class MediaInfoPage : UserControl
{
    public MediaInfoPageViewModel ViewModel { get; }

    public MediaInfoPage() : this(App.GetService<MediaInfoPageViewModel>())
    {
    }

    public MediaInfoPage(MediaInfoPageViewModel viewModel)
    {
        DataContext = ViewModel = viewModel;
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.GetFileNames() is { } fileNames)
        {
            if (fileNames.Any())
            {
                ViewModel.Reload(fileNames.First());
            }
        }
    }
}
