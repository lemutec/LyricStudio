using FluentAvalonia.UI.Controls;
using System;

namespace LyricStudio.Services;

public interface INavigationService
{
    public void SetFrame(Frame frame);

    public void SetNavigationView(NavigationView navigationView);

    public void Navigate(Type navigateTo, object? extraData = null);

    public void Navigate(NavigationViewItemInvokedEventArgs e);
}
