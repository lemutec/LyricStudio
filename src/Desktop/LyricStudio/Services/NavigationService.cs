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
        NavigationView.ItemInvoked += NavigationView_ItemInvoked;
    }

    private void NavigationView_ItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        Navigate((e.InvokedItemContainer as NavigationViewItem).Tag as Type);
    }

    public void Navigate(Type navigateTo, object? extraData = null)
    {
        {
            var types = NavigationView.MenuItems.Select(o => (o as NavigationViewItem).Tag as Type);

            if (types.Contains(navigateTo))
            {
                NavigationView.SelectedItem = NavigationView.MenuItems[types.IndexOf(navigateTo)];
            }
        }
        {
            var types = NavigationView.FooterMenuItems.Select(o => (o as NavigationViewItem).Tag as Type);

            if (types.Contains(navigateTo))
            {
                NavigationView.SelectedItem = NavigationView.FooterMenuItems[types.IndexOf(navigateTo)];
            }
        }
        Frame?.Navigate(navigateTo, extraData, ContentTransitions);
    }

    public void Navigate(NavigationViewItemInvokedEventArgs e)
    {
        throw new NotImplementedException();
    }
}
