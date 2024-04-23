using System;

namespace LyricStudio.Core.Configuration;

public static class Config
{
    public static IConfigurationImpl Configer { get; set; } = null!;

    public static event EventHandler<SettingChangedEventArgs>? SettingChanged
    {
        add => Configer.SettingChanged += value;
        remove => Configer.SettingChanged -= value;
    }

    public static event EventHandler<SettingChangingEventArgs>? SettingChanging
    {
        add => Configer.SettingChanging += value;
        remove => Configer.SettingChanging -= value;
    }

    public static void Load(string path)
    {
        Configer?.Load(path);
    }

    public static void Save(string path)
    {
        Configer?.Save(path);
    }

    public static object? GetRawValue(SettingKey key)
    {
        return Configer?.GetRawValue(key);
    }

    public static void ResetValue(SettingKey key)
    {
        Configer?.ResetValue(key);
    }

    public static void SetValue(SettingKey key, object value)
    {
        Configer?.SetValue(key, value);
    }
}
