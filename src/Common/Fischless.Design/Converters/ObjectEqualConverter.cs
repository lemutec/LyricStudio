using System.Globalization;

namespace Fischless.Design.Converters;

public class ObjectEqualsConverter : SingletonConverterBase<ObjectEqualsConverter>
{
    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(parameter) ?? false;
    }
}
