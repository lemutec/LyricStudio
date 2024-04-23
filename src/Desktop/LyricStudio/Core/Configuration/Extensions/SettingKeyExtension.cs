namespace LyricStudio.Core.Configuration;

public static class SettingKeyExtension
{
    public static T Get<T>(this SettingKey<T> settingKey)
    {
        return Config.Configer.GetValueOrDefault(settingKey);
    }

    public static void Set<T>(this SettingKey<T> settingKey, T value)
    {
        Config.Configer.SetValue(settingKey, value);
    }
}
