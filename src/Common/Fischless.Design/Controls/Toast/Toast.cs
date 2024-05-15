﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;
using Avalonia.Controls.Primitives.PopupPositioning;

namespace Fischless.Design.Controls;

public static class Toast
{
    public static void Info(string message, ToastLocation location = ToastLocation.TopCenter, Thickness offsetMargin = default, int time = ToastConfig.NormalTime)
        => _ = ShowAsync(null!, message, new ToastConfig(ToastIcon.Information, location, time) { Margin = offsetMargin });

    public static void Warn(string message, ToastLocation location = ToastLocation.TopCenter, Thickness offsetMargin = default, int time = ToastConfig.NormalTime)
        => _ = ShowAsync(null!, message, new ToastConfig(ToastIcon.Warning, location, time) { Margin = offsetMargin });

    public static void Error(string message, ToastLocation location = ToastLocation.TopCenter, Thickness offsetMargin = default, int time = ToastConfig.NormalTime)
        => _ = ShowAsync(null!, message, new ToastConfig(ToastIcon.Error, location, time) { Margin = offsetMargin });

    public static void Success(string message, ToastLocation location = ToastLocation.TopCenter, Thickness offsetMargin = default, int time = ToastConfig.NormalTime)
        => _ = ShowAsync(null!, message, new ToastConfig(ToastIcon.Success, location, time) { Margin = offsetMargin });

    public static void Question(string message, ToastLocation location = ToastLocation.TopCenter, Thickness offsetMargin = default, int time = ToastConfig.NormalTime)
        => _ = ShowAsync(null!, message, new ToastConfig(ToastIcon.Question, location, time) { Margin = offsetMargin });

    public static async Task ShowAsync(string message, ToastConfig options = null)
        => await ShowAsync(null!, message, options);

    public static async Task ShowAsync(Control? parent, string message, ToastConfig options = null)
    {
        parent ??= GetActiveWindow();
        options ??= new ToastConfig();

        await ShowCoreAsync(parent, message, TimeSpan.FromMilliseconds(options.Time), options.Icon, options.Location, options.Margin);
    }

    private static async Task ShowCoreAsync(Control parent, string message, TimeSpan displayTime, ToastIcon icon, ToastLocation location, Thickness margin)
    {
        PlacementMode placement = location switch
        {
            ToastLocation.Center => PlacementMode.Center,
            ToastLocation.Left => PlacementMode.Left,
            ToastLocation.Right => PlacementMode.Right,
            ToastLocation.TopLeft => PlacementMode.TopEdgeAlignedLeft,
            ToastLocation.TopCenter => PlacementMode.AnchorAndGravity,
            ToastLocation.TopRight => PlacementMode.TopEdgeAlignedRight,
            ToastLocation.BottomLeft => PlacementMode.BottomEdgeAlignedLeft,
            ToastLocation.BottomCenter => PlacementMode.AnchorAndGravity,
            ToastLocation.BottomRight => PlacementMode.BottomEdgeAlignedRight,
            _ => throw new ArgumentOutOfRangeException(nameof(ToastConfig.Location)),
        };

        ToastControl toast = new()
        {
            Message = message,
            ImageGlyph = icon switch
            {
                ToastIcon.None => null,
                ToastIcon.Error => FontSymbols.Error,
                ToastIcon.Information => FontSymbols.Info,
                ToastIcon.Warning => FontSymbols.Warning,
                ToastIcon.Question => FontSymbols.StatusCircleQuestionMark,
                ToastIcon.Success => FontSymbols.CheckMark,
                _ => throw new NotSupportedException(),
            },
        };

        Popup popup = new()
        {
            Placement = PlacementMode.AnchorAndGravity,
            PlacementTarget = parent,
            HorizontalOffset = margin.Left,
            VerticalOffset = margin.Top,
            Child = toast,
            IsLightDismissEnabled = false,
        };

        if (location == ToastLocation.TopCenter)
        {
            popup.PlacementAnchor = PopupAnchor.Top;
            popup.PlacementGravity = PopupGravity.None;
            popup.PlacementConstraintAdjustment = PopupPositionerConstraintAdjustment.SlideX | PopupPositionerConstraintAdjustment.SlideY;
            popup.PlacementRect = new Rect(0, 0, parent.Bounds.Width, parent.Bounds.Height);
        }
        else if (location == ToastLocation.BottomCenter)
        {
            popup.PlacementAnchor = PopupAnchor.Bottom;
            popup.PlacementGravity = PopupGravity.None;
            popup.PlacementConstraintAdjustment = PopupPositionerConstraintAdjustment.SlideX | PopupPositionerConstraintAdjustment.SlideY;
            popup.PlacementRect = new Rect(0, 0, parent.Bounds.Width, parent.Bounds.Height);
        }

#if false // Apply Mica Backdrop
        popup.Opened += (object? sender, EventArgs e) =>
        {
            if (sender is Popup popup)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (popup.Child is Control childControl)
                    {
                        nint hwnd = WindowInteropHelper.GetHwndForControl(childControl);

                        if (hwnd != IntPtr.Zero)
                        {
                            _ = WindowBackdrop.ApplyBackdrop(hwnd, WindowBackdropType.Mica);
                        }
                    }
                }, DispatcherPriority.Background);
            }
        };
#endif

        ((ISetLogicalParent)popup).SetParent(parent);

        Animation fadeInAnimation = new()
        {
            Duration = TimeSpan.FromMilliseconds(250),
            Easing = new SineEaseInOut(),
            Children =
            {
                new KeyFrame()
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 0d)
                    }
                },
                new KeyFrame()
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 1d)
                    }
                }
            }
        };

        Animation fadeOutAnimation = new()
        {
            Duration = TimeSpan.FromMilliseconds(150),
            Easing = new SineEaseInOut(),
            Children =
            {
                new KeyFrame()
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 1d)
                    }
                },
                new KeyFrame()
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 0d)
                    }
                }
            }
        };

        popup.IsOpen = true;
        await fadeInAnimation.RunAsync(toast);

        await Task.Delay(displayTime);

        await fadeOutAnimation.RunAsync(toast);
        popup.IsOpen = false;
    }

    private static Window? GetActiveWindow()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.Windows.FirstOrDefault(window => window.IsActive && window.ShowActivated) ?? desktop.MainWindow;
        }

        return null;
    }
}
