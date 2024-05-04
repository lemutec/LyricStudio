using Avalonia;
using System.Globalization;

namespace Fischless.Design.Converters;

public class TimeSpanToStringConverter : SingletonConverterBase<TimeSpanToStringConverter>
{
    protected const string DefaultFormat = @"g";
    protected const string DefaultMinValueString = "";

    public static readonly AvaloniaProperty FormatProperty = AvaloniaProperty.Register<TimeSpanToStringConverter, string>(
        "Format",
        DefaultFormat);

    public static readonly AvaloniaProperty MinValueStringProperty = AvaloniaProperty.Register<TimeSpanToStringConverter, string>(
        "MinValueString",
        DefaultMinValueString);

    /// <summary>
    /// The timespan format property.
    /// Standard date and time format strings: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings
    /// </summary>
    public string Format
    {
        get => (string)this.GetValue(FormatProperty);
        set => this.SetValue(FormatProperty, value);
    }

    public string MinValueString
    {
        get => (string)this.GetValue(MinValueStringProperty);
        set => this.SetValue(MinValueStringProperty, value);
    }

    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.MinValue)
            {
                return this.MinValueString;
            }

            return timeSpan.ToString(this.Format, culture);
        }

        return UnsetValue;
    }

    protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value != null)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan;
            }

            if (value is string str)
            {
                _ = TimeSpan.TryParse(str, out var resultTimeSpan);
                return resultTimeSpan;
            }
        }
        return null;
    }
}
