﻿using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Vanara.PInvoke;

namespace Fischless.Win32;

[SupportedOSPlatform("Windows")]
internal static class DwmApi
{
    public static HRESULT DwmSetWindowAttribute(HWND hWnd, DWMWINDOWATTRIBUTE dwAttribute, int pvAttribute, int cbAttribute)
    {
        nint pvAttributePtr = Marshal.AllocHGlobal(sizeof(int));
        Marshal.WriteInt32(pvAttributePtr, pvAttribute);

        try
        {
            return Vanara.PInvoke.DwmApi.DwmSetWindowAttribute(
                hWnd,
                (Vanara.PInvoke.DwmApi.DWMWINDOWATTRIBUTE)dwAttribute,
                pvAttributePtr,
                Marshal.SizeOf(typeof(int))
            );
        }
        finally
        {
            Marshal.FreeHGlobal(pvAttributePtr);
        }
    }

    /// <summary>
    /// DWMWINDOWATTRIBUTE enumeration. (dwmapi.h)
    /// <para><see href="https://github.com/electron/electron/issues/29937"/></para>
    /// </summary>
    [Flags]
    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_NCRENDERING_ENABLED = 1,
        DWMWA_NCRENDERING_POLICY,
        DWMWA_TRANSITIONS_FORCEDISABLED,
        DWMWA_ALLOW_NCPAINT,
        DWMWA_CAPTION_BUTTON_BOUNDS,
        DWMWA_NONCLIENT_RTL_LAYOUT,
        DWMWA_FORCE_ICONIC_REPRESENTATION,
        DWMWA_FLIP3D_POLICY,
        DWMWA_EXTENDED_FRAME_BOUNDS,
        DWMWA_HAS_ICONIC_BITMAP,
        DWMWA_DISALLOW_PEEK,
        DWMWA_EXCLUDED_FROM_PEEK,
        DWMWA_CLOAK,
        DWMWA_CLOAKED,
        DWMWA_FREEZE_REPRESENTATION,
        DWMWA_USE_HOSTBACKDROPBRUSH,
        DWMWA_USE_IMMERSIVE_DARK_MODE_OLD = 19,
        DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
        DWMWA_WINDOW_CORNER_PREFERENCE = 33,
        DWMWA_BORDER_COLOR,
        DWMWA_CAPTION_COLOR,
        DWMWA_TEXT_COLOR,
        DWMWA_VISIBLE_FRAME_BORDER_THICKNESS,
        DWMWA_SYSTEMBACKDROP_TYPE,
        DWMWA_MICA_EFFECT = 1029,
    }
}
