using Avalonia.Controls;
using Avalonia.Platform;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;

namespace Fischless.Win32;

/// <summary>
/// Implements Avalon WindowInteropHelper classes, which helps
/// interop b/w legacy and Avalon Window.
/// Ported from https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Interop/WindowInteropHelper.cs
/// </summary>
[SupportedOSPlatform("Windows")]
public sealed class WindowInteropHelper
{
    private Window _window;

    /// <summary>
    /// Get the Handle of the window
    /// </summary>
    /// <remarks>
    ///     Callers must have UIPermission(UIPermissionWindow.AllWindows) to call this API.
    /// </remarks>
    public nint Handle { get; } = IntPtr.Zero;

    /// <summary>
    /// Get/Set the Owner handle of the window
    /// </summary>
    /// <remarks>
    ///     Callers must have UIPermission(UIPermissionWindow.AllWindows) to call this API.
    /// </remarks>
    public nint Owner
    {
        get
        {
            Debug.Assert(_window != null, "Cannot be null since we verify in the constructor");
            return new WindowInteropHelper(_window.Owner.PlatformImpl).Handle;
        }
    }

    public WindowInteropHelper(Window window) : this(window.PlatformImpl)
    {
        _window = window;
    }

    private WindowInteropHelper(IWindowBaseImpl? platformImpl)
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            if (platformImpl.GetType().GetProperty("Hwnd", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(platformImpl) is nint handle)
            {
                Handle = handle;
            }
        }
    }
}
