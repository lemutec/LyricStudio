using System.Runtime.Versioning;

namespace Fischless.Win32;

/// <summary>
/// Theme in which an application using WPF UI is displayed.
/// </summary>
[SupportedOSPlatform("Windows")]
public enum ApplicationTheme
{
    /// <summary>
    /// Unknown application theme.
    /// </summary>
    Unknown,

    /// <summary>
    /// Dark application theme.
    /// </summary>
    Dark,

    /// <summary>
    /// Light application theme.
    /// </summary>
    Light,

    /// <summary>
    /// High contract application theme.
    /// </summary>
    HighContrast,
}
