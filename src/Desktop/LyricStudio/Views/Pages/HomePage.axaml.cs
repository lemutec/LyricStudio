﻿using Avalonia;
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

    private void OnTimeClickBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border timeClickBar)
        {
            Point relativePoint = e.GetPosition(timeClickBar);
            double relativeX = relativePoint.X;
            double relativePosition = relativeX / timeClickBar.Bounds.Width;

            ViewModel.SeekInPosition(relativePosition);
        }
    }

    private void OnLyricListBoxDoubleTapped(object? sender, TappedEventArgs e)
    {
        ViewModel.PlaySeekLyric();
    }

    private void OnLyricListBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
        {
            ViewModel.PlaySeekLyric();
        }
    }
}
