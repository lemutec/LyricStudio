using System.Reflection;

namespace LyricStudio;

internal static class AppConfig
{
    public static string PackName => "LyricStudio";
    public static string LogFolder { get; internal set; }
    public static string SettingsFile { get; internal set; }
    public static string Website => "https://github.com/lemutec/LyricStudio";
    public static string AppVersion => typeof(AppConfig).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
}
