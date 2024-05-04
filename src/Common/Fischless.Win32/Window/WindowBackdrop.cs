using Avalonia.Controls;
using Avalonia.Media;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Vanara.PInvoke;

namespace Fischless.Win32;

[SupportedOSPlatform("Windows")]
public static class WindowBackdrop
{
    private static bool IsSupported(WindowBackdropType backdropType)
    {
        return backdropType switch
        {
            WindowBackdropType.Auto => OsVersionHelper.IsWindows11_22523,
            WindowBackdropType.Tabbed => OsVersionHelper.IsWindows11_22523,
            WindowBackdropType.Mica => OsVersionHelper.IsWindows11_OrGreater,
            WindowBackdropType.Acrylic => OsVersionHelper.IsWindows7_OrGreater,
            WindowBackdropType.None => OsVersionHelper.IsWindows11_OrGreater,
            _ => false
        };
    }

    public static bool PrepareBackground(this Window window, WindowBackdropType backdropType = WindowBackdropType.Mica)
    {
        if (!IsSupported(backdropType))
        {
            return false;
        }

        if (window is null)
        {
            return false;
        }

        if (ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark)
        {
            window.Background = new SolidColorBrush(Color.FromRgb(0x27, 0x27, 0x27));
        }
        else
        {
            window.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
        }

        return true;
    }

    public static bool ApplyBackdrop(this Window window, WindowBackdropType backdropType = WindowBackdropType.Mica, ApplicationTheme theme = ApplicationTheme.Unknown)
    {
        if (!IsSupported(backdropType))
        {
            return false;
        }

        if (window is null)
        {
            return false;
        }

        if (window.IsLoaded)
        {
            nint hWnd = new WindowInteropHelper(window).Handle;

            if (hWnd == 0x00)
            {
                return false;
            }

            window.Background = Brushes.Transparent;
            return ApplyBackdrop(hWnd, backdropType, theme);
        }

        window.Loaded += (sender, _) =>
        {
            nint hWnd = new WindowInteropHelper(sender as Window ?? null).Handle;

            if (hWnd == 0x00)
            {
                return;
            }

            window.Background = Brushes.Transparent;
            ApplyBackdrop(hWnd, backdropType, theme);
        };

        return true;
    }

    public static bool ApplyBackdrop(nint hWnd, WindowBackdropType backdropType = WindowBackdropType.Mica, ApplicationTheme theme = ApplicationTheme.Unknown)
    {
        if (!IsSupported(backdropType))
        {
            return false;
        }

        if (hWnd == 0x00 || !User32.IsWindow(hWnd))
        {
            return false;
        }

        switch (theme)
        {
            case ApplicationTheme.Unknown:
                if (ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark)
                {
                    WindowDarkMode.ApplyWindowDarkMode(hWnd);
                }
                else
                {
                    WindowDarkMode.RemoveWindowDarkMode(hWnd);
                }
                break;

            case ApplicationTheme.Dark:
                WindowDarkMode.ApplyWindowDarkMode(hWnd);
                break;

            case ApplicationTheme.Light:
            case ApplicationTheme.HighContrast:
                WindowDarkMode.RemoveWindowDarkMode(hWnd);
                break;
        }

        var wtaOptions = new UxTheme.WTA_OPTIONS()
        {
            Flags = UxTheme.WTNCA.WTNCA_NODRAWCAPTION,
            Mask = (uint)UxTheme.ThemeDialogTextureFlags.ETDT_VALIDBITS,
        };

#if false // This is not needed for Avalonia UI Window
        UxTheme.SetWindowThemeAttribute(
            hWnd,
            UxTheme.WINDOWTHEMEATTRIBUTETYPE.WTA_NONCLIENT,
            wtaOptions,
            (uint)Marshal.SizeOf(typeof(UxTheme.WTA_OPTIONS))
        );
#endif

        var dwmApiResult = DwmApi.DwmSetWindowAttribute(
            hWnd,
            DwmApi.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
            (int)(backdropType switch
            {
                WindowBackdropType.Auto => Vanara.PInvoke.DwmApi.DWM_SYSTEMBACKDROP_TYPE.DWMSBT_AUTO,
                WindowBackdropType.Mica => Vanara.PInvoke.DwmApi.DWM_SYSTEMBACKDROP_TYPE.DWMSBT_MAINWINDOW,
                WindowBackdropType.Acrylic => Vanara.PInvoke.DwmApi.DWM_SYSTEMBACKDROP_TYPE.DWMSBT_TRANSIENTWINDOW,
                WindowBackdropType.Tabbed => Vanara.PInvoke.DwmApi.DWM_SYSTEMBACKDROP_TYPE.DWMSBT_TABBEDWINDOW,
                _ => Vanara.PInvoke.DwmApi.DWM_SYSTEMBACKDROP_TYPE.DWMSBT_NONE,
            }),
            Marshal.SizeOf(typeof(int))
        );

        return dwmApiResult == HRESULT.S_OK;
    }

    public static bool RemoveBackdrop(Window window)
    {
        if (window == null)
        {
            return false;
        }

        nint hWnd = new WindowInteropHelper(window).Handle;

        return RemoveBackdrop(hWnd);
    }

    public static bool RemoveBackdrop(nint hWnd)
    {
        if (hWnd == 0x00 || !User32.IsWindow(hWnd))
        {
            return false;
        }

        _ = DwmApi.DwmSetWindowAttribute(
            hWnd,
            DwmApi.DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT,
            0x0,
            Marshal.SizeOf(typeof(int))
        );

        _ = DwmApi.DwmSetWindowAttribute(
            hWnd,
            DwmApi.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
            (int)Vanara.PInvoke.DwmApi.DWM_SYSTEMBACKDROP_TYPE.DWMSBT_NONE,
            Marshal.SizeOf(typeof(int))
        );

        return true;
    }
}
