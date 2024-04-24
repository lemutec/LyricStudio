using Avalonia.Markup.Xaml.MarkupExtensions;
using Fischless.Globalization.Properties;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;

namespace Fischless.Globalization;

public static partial class MuiLanguage
{
    public static string MuiLanguageName { get; private set; } = string.Empty;

    public static ResourceManager ResourceManager { get; } = new("Fischless.Globalization.Properties.Resources", typeof(Resources).Assembly);

    public static string DetectLanguage() => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
    {
        "zh" => "zh-CN",
        "ja" => "ja",
        "en" or _ => "en",
    };

    public static void SetupLanguage(string lang)
    {
        if (!string.IsNullOrWhiteSpace(lang))
        {
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
        }
        _ = SetLanguage();
    }

    public static bool SetLanguage() => SetLanguage(DetectLanguage());

    public static bool SetLanguage(string name = "en")
    {
        MuiLanguageName = name;

        if (!string.IsNullOrWhiteSpace(name))
        {
            CultureInfo cultureInfo = new(name);

            if (cultureInfo.NumberFormat.NumberDecimalSeparator != ".")
            {
                Debug.WriteLine($"[MuiLanguageManager] Protect number decimal seprator from diff culture '{cultureInfo.TwoLetterISOLanguageName}', so set culture to 'en'.");
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = cultureInfo;
            }
        }

#if !PREVIEW
        try
        {
            I18NExtension.Culture = new CultureInfo(name);
        }
        catch (Exception e)
        {
            _ = e;
        }
#else
        Culture = new CultureInfo(name);
#endif
        return false;
    }

    public static string Mui(string key)
    {
        try
        {
#if !PREVIEW
            return I18NExtension.Translate(key);
#else
            return Translate(key);
#endif
        }
        catch (Exception e)
        {
            _ = e;
        }
        return null!;
    }

    public static string Mui(CultureInfo cultureInfo, string key)
    {
        return ResourceManager.GetString(key, cultureInfo);
    }

    public static string Mui(string key, params object[] args)
    {
        return string.Format(Mui(key)?.ToString(), args);
    }
}

public enum LanguageIndex
{
    [Description("zh")]
    Chinese,

    [Description("ja")]
    Japanese,

    [Description("en")]
    English,
}
