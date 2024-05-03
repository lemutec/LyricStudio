using Avalonia.Data.Converters;
using Avalonia.Metadata;
using System.Globalization;

namespace Fischless.Design.Converters;

/// <summary>
/// Value converters which aggregates the results of a sequence of converters: Converter1 >> Converter2 >> Converter3
/// The output of converter N becomes the input of converter N+1.
/// </summary>
public class ValueConverterGroup : SingletonConverterBase<ValueConverterGroup>
{
    [Content]
    public List<IValueConverter> Converters { get; set; } = [];

    protected override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (this.Converters is IEnumerable<IValueConverter> converters)
        {
            var language = culture;
            return converters.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, language));
        }

        return UnsetValue;
    }

    protected override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (this.Converters is IEnumerable<IValueConverter> converters)
        {
            var language = culture;
            return converters.Reverse<IValueConverter>().Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, language));
        }

        return UnsetValue;
    }
}
