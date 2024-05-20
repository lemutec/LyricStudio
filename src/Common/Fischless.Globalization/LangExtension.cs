using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using System.ComponentModel;

namespace Fischless.Globalization;

/// <summary>
/// namespace Avalonia.Markup.Xaml.MarkupExtensions;
/// </summary>
[Obsolete(nameof(I18NExtension))]
public class LangExtension : MarkupExtension
{
    [DefaultValue(null)]
    public object? Key { get; set; }

    public LangExtension()
    {
    }

    public LangExtension(string key)
    {
        Key = key;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return MuiLanguage.Mui(Key?.ToString() ?? string.Empty);
    }
}
