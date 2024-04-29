using LyricStudio.Core.Configuration;

namespace LyricStudio.Models;

internal static class ConfigurationKeys
{
    public static readonly SettingKey<string> Language = new(nameof(Language), defaultValue: string.Empty);
    public static readonly SettingKey<double> Volume = new(nameof(Volume), defaultValue: 1d);
}
