using Fischless.Design.Controls;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using LyricStudio.Views;
using System;
using System.Linq;

namespace LyricStudio.Services;

public sealed class NavigationService : INavigationService
{
    public NavigationView NavigationView { get; set; } = null!;
    public Frame Frame { get; private set; } = null!;
    public Type NavigateFrom { get; set; } = typeof(HomePage);

    /// <summary>
    /// <see cref="DrillInNavigationTransitionInfo"/>
    /// <see cref="EntranceNavigationTransitionInfo"/>
    /// <see cref="SuppressNavigationTransitionInfo"/>
    /// </summary>
    public NavigationTransitionInfo ContentTransitions { get; set; } = new SuppressNavigationTransitionInfo();

    public void SetFrame(Frame frame)
        => Frame = frame;

    public void SetNavigationView(NavigationView navigationView)
    {
        NavigationView = navigationView;
        NavigationView.SelectionChanged += NavigationView_SelectionChanged;
    }

    public void Navigate(Type navigateTo, object? extraData = null)
    {
        var types = NavigationView.MenuItems.Select(o => (o as NavigationViewItem).Tag as Type);
        NavigationView.SelectedItem = NavigationView.MenuItems[types.IndexOf(navigateTo)];
        Frame?.Navigate(navigateTo, extraData, ContentTransitions);
    }

    public void Navigate(NavigationViewItemInvokedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NavigationView_SelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        Navigate((e.SelectedItem as NavigationViewItem).Tag as Type);
    }
}
