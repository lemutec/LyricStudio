using System.Globalization;

namespace Fischless.Design.Converters;

public sealed class IsNullOrEmptyStringConverter : SingletonConverterBase<IsNullOrEmptyStringConverter>
{
    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return string.IsNullOrEmpty(value?.ToString());
    }
}

public sealed class IsNullOrEmptyStringInvertConverter : SingletonConverterBase<IsNullOrEmptyStringInvertConverter>
{
    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value?.ToString());
    }
}
