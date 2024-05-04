using Avalonia;
using System.Collections;
using System.Globalization;

namespace Fischless.Design.Converters;

public class IsCountEmptyEmptyConverter : SingletonConverterBase<IsEmptyConverter>
{
    public static readonly AvaloniaProperty IsInvertedProperty = AvaloniaProperty.Register<IsEmptyConverter, bool>(
        "IsInverted",
        false);

    public bool IsInverted
    {
        get => (bool)this.GetValue(IsInvertedProperty)!;
        set => this.SetValue(IsInvertedProperty, value);
    }

    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable enumerable)
        {
            if (((dynamic)enumerable).Count > 0)
            {
                return false ^ this.IsInverted;
            }
            else
            {
                return true ^ this.IsInverted;
            }
        }
        else
        {
            if (value is int cint)
            {
                if (cint > 0)
                {
                    return false ^ this.IsInverted;
                }
                else
                {
                    return true ^ this.IsInverted;
                }
            }
            if (value is uint cuint)
            {
                if (cuint > 0)
                {
                    return false ^ this.IsInverted;
                }
                else
                {
                    return true ^ this.IsInverted;
                }
            }
        }

        return true ^ this.IsInverted;
    }
}
