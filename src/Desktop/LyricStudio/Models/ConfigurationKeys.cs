using LyricStudio.Core.Configuration;

namespace LyricStudio.Models;

internal static class ConfigurationKeys
{
    public static readonly SettingKey<string> Language = new(nameof(Language), defaultValue: string.Empty);
    public static readonly SettingKey<double> Volume = new(nameof(Volume), defaultValue: 1d);
    public static readonly SettingKey<bool> IsUseTwoDigitTimeCode = new(nameof(IsUseTwoDigitTimeCode), defaultValue: false);
    public static readonly SettingKey<int> FlagTimeOffset = new(nameof(FlagTimeOffset), defaultValue: 150);
    public static readonly SettingKey<double> LongShiftSeconds = new(nameof(LongShiftSeconds), defaultValue: 5d);
    public static readonly SettingKey<double> ShortShiftSeconds = new(nameof(ShortShiftSeconds), defaultValue: 2d);
}
