using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Vanara.PInvoke;

namespace Fischless.Win32;

[SupportedOSPlatform("Windows")]
public static class WindowDarkMode
{
    public static bool ApplyWindowDarkMode(nint hWnd)
    {
        if (hWnd == 0x00 || !User32.IsWindow(hWnd))
        {
            return false;
        }

        var dwAttribute = DwmApi.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE;

        if (!OsVersionHelper.IsWindows11_22523_OrGreater)
        {
            dwAttribute = DwmApi.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_OLD;
        }

        _ = DwmApi.DwmSetWindowAttribute(
            hWnd,
            dwAttribute,
            0x1,
            Marshal.SizeOf(typeof(int))
        );

        return true;
    }

    public static bool RemoveWindowDarkMode(nint handle)
    {
        if (handle == 0x00 || !User32.IsWindow(handle))
        {
            return false;
        }

        var dwAttribute = DwmApi.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE;

        if (!OsVersionHelper.IsWindows11_22523_OrGreater)
        {
            dwAttribute = DwmApi.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_OLD;
        }

        _ = DwmApi.DwmSetWindowAttribute(
            handle,
            dwAttribute,
            0x0,
            Marshal.SizeOf(typeof(int))
        );
        return true;
    }
}
