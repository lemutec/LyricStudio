using Avalonia;
using Fischless.Design.Converters.Extensions;
using Fischless.Design.Converters.Services;
using System.Globalization;

namespace Fischless.Design.Converters;

/// <summary>
/// Converts a <seealso cref="DateTime"/> value to string using formatting specified in <seealso cref="DefaultFormat"/>.
/// </summary>
public class DateTimeToStringConverter : SingletonConverterBase<DateTimeToStringConverter>
{
    protected const string DefaultFormat = "g";
    protected const string DefaultMinValueString = "";

    private readonly ITimeZoneInfo timeZone;

    public DateTimeToStringConverter() : this(SystemTimeZoneInfo.Current)
    {
    }

    internal DateTimeToStringConverter(ITimeZoneInfo timeZone)
    {
        this.timeZone = timeZone;
    }

    public static readonly AvaloniaProperty FormatProperty = AvaloniaProperty.Register<DateTimeToStringConverter, string>(
        nameof(Format),
        DefaultFormat);

    public static readonly AvaloniaProperty MinValueStringProperty = AvaloniaProperty.Register<DateTimeToStringConverter, string>(
        nameof(MinValueString),
        DefaultMinValueString);

    /// <summary>
    /// The datetime format property.
    /// Standard date and time format strings: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
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

    protected override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                return this.MinValueString;
            }

            var localDateTime = dateTime.WithTimeZone(this.timeZone.Local);
            return localDateTime.ToString(this.Format, culture);
        }

        return UnsetValue;
    }

    protected override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value != null)
        {
            if (value is DateTime dateTime)
            {
                return dateTime;
            }

            if (value is string str)
            {
                if (DateTime.TryParse(str, out var parsedDateTime))
                {
                    return parsedDateTime.WithTimeZone(this.timeZone.Utc);
                }
            }
        }

        return UnsetValue;
    }
}
