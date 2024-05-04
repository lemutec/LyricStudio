using Avalonia;
using System.Collections;
using System.Globalization;

namespace Fischless.Design.Converters;

public class IsEmptyConverter : SingletonConverterBase<IsEmptyConverter>
{
    public static readonly AvaloniaProperty IsInvertedProperty = AvaloniaProperty.Register<IsEmptyConverter, bool>(
        "IsInverted",
        false);

    public bool IsInverted
    {
        get => (bool)this.GetValue(IsInvertedProperty);
        set => this.SetValue(IsInvertedProperty, value);
    }

    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable enumerable)
        {
            var hasAtLeastOne = enumerable.GetEnumerator().MoveNext();
            return (hasAtLeastOne == false) ^ this.IsInverted;
        }

        return true ^ this.IsInverted;
    }
}
