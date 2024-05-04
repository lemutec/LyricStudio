using Microsoft.Win32;
using System.Runtime.Versioning;

namespace Fischless.Win32;

public static class ApplicationThemeManager
{
    [SupportedOSPlatform("Windows")]
    public static ApplicationTheme GetAppTheme()
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        object? registryValueObject = key?.GetValue("AppsUseLightTheme");

        if (registryValueObject == null)
        {
            return ApplicationTheme.Light;
        }

        var registryValue = (int)registryValueObject;

        return registryValue > 0 ? ApplicationTheme.Light : ApplicationTheme.Dark;
    }
}
