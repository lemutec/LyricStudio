using Avalonia.Controls;
using Avalonia.Threading;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Vanara.PInvoke;

namespace Fischless.Win32;

/// <summary>
/// Uses DWMWA_CLOAK to hide a Win32 HWND from the user while Avalonia finishes its first
/// GPU frame, avoiding the "empty frame / transparent client area" flash on Show.
/// </summary>
[SupportedOSPlatform("Windows")]
public static class WindowCloak
{
    private const int UncloakFrameCount = 2;
    private static readonly TimeSpan UncloakFallbackDelay = TimeSpan.FromMilliseconds(500);

    public static bool ApplyCloak(Window? window, bool cloaked)
    {
        if (!OsVersionHelper.IsWindows8_OrGreater || window is null)
        {
            return false;
        }

        nint hWnd = new WindowInteropHelper(window).Handle;
        return ApplyCloak(hWnd, cloaked);
    }

    public static bool ApplyCloak(nint hWnd, bool cloaked)
    {
        if (!OsVersionHelper.IsWindows8_OrGreater)
        {
            return false;
        }

        if (hWnd == 0x00 || !User32.IsWindow(hWnd))
        {
            return false;
        }

        var result = DwmApi.DwmSetWindowAttribute(
            hWnd,
            DwmApi.DWMWINDOWATTRIBUTE.DWMWA_CLOAK,
            cloaked ? 0x1 : 0x0,
            Marshal.SizeOf(typeof(int))
        );

        return result == HRESULT.S_OK;
    }

    /// <summary>
    /// Uncloaks after a couple of animation frames (with a timeout fallback).
    /// </summary>
    public static void ScheduleUncloak(Window? window)
    {
        if (!OsVersionHelper.IsWindows8_OrGreater || window is null)
        {
            return;
        }

        var frames = 0;

        void OnFrame(TimeSpan _)
        {
            if (++frames < UncloakFrameCount)
            {
                window.RequestAnimationFrame(OnFrame);
                return;
            }

            ApplyCloak(window, cloaked: false);
        }

        window.RequestAnimationFrame(OnFrame);

        DispatcherTimer.RunOnce(
            () => ApplyCloak(window, cloaked: false),
            UncloakFallbackDelay);
    }
}
