using Avalonia;
using System.Globalization;

namespace Fischless.Design.Converters;

[Obsolete("StringLengthToBoolConverter has been renamed to StringIsNotNullOrEmptyConverter. Please use StringIsNotNullOrEmptyConverter. StringLengthToBoolConverter will be removed in future releases.")]
public class StringLengthToBoolConverter : StringIsNotNullOrEmptyConverter
{
}

public class StringIsNotNullOrEmptyConverter : StringIsNullOrEmptyConverter
{
    public StringIsNotNullOrEmptyConverter()
    {
        this.IsInverted = true;
    }
}

public class StringIsNullOrEmptyConverter : SingletonConverterBase<StringIsNotNullOrEmptyConverter>
{
    public static readonly AvaloniaProperty IsInvertedProperty = AvaloniaProperty.Register<StringIsNullOrEmptyConverter, bool>(
        nameof(IsInverted),
        false);

    public bool IsInverted
    {
        get => (bool)this.GetValue(IsInvertedProperty);
        set => this.SetValue(IsInvertedProperty, value);
    }

    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (this.IsInverted)
        {
            return !string.IsNullOrEmpty(value as string);
        }

        return string.IsNullOrEmpty(value as string);
    }
}
