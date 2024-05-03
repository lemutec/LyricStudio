using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Fischless.Design.Converters;

public class StringToDecimalConverter : SingletonConverterBase<StringToDecimalConverter>
{
    private static readonly NumberStyles DefaultNumberStyles = NumberStyles.Any;

    [SuppressMessage("Style", "IDE0019:Use pattern matching")]
    [SuppressMessage("Style", "IDE0018:Inline variable declaration")]
    protected override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var dec = value as decimal?;
        if (dec != null)
        {
            return dec.Value.ToString("G", culture ?? CultureInfo.InvariantCulture);
        }

        var str = value as string;
        if (str != null)
        {
            decimal result;
            if (decimal.TryParse(str, DefaultNumberStyles, culture ?? CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            return result;
        }

        return UnsetValue;
    }

    protected override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return this.Convert(value, targetType, parameter, culture);
    }
}
