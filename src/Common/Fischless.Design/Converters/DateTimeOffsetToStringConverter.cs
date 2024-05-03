using Avalonia;
using Fischless.Design.Converters.Extensions;
using Fischless.Design.Converters.Services;
using System.Globalization;

namespace Fischless.Design.Converters;

/// <summary>
/// Converts a <seealso cref="DateTimeOffset"/> value to string using formatting specified in <seealso cref="DefaultFormat"/>.
/// </summary>
public class DateTimeOffsetToStringConverter : SingletonConverterBase<DateTimeOffsetToStringConverter>
{
    protected const string DefaultFormat = "g";
    protected const string DefaultMinValueString = "";

    private readonly ITimeZoneInfo timeZone;

    public DateTimeOffsetToStringConverter() : this(SystemTimeZoneInfo.Current)
    {
    }

    internal DateTimeOffsetToStringConverter(ITimeZoneInfo timeZone)
    {
        this.timeZone = timeZone;
    }

    public static readonly AvaloniaProperty FormatProperty = AvaloniaProperty.Register<DateTimeOffsetToStringConverter, string>(
        nameof(Format),
        DefaultFormat);

    public static readonly AvaloniaProperty MinValueStringProperty = AvaloniaProperty.Register<DateTimeOffsetToStringConverter, string>(
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
        if (value is DateTimeOffset dateTimeOffset)
        {
            if (dateTimeOffset == DateTimeOffset.MinValue)
            {
                return this.MinValueString;
            }

            return dateTimeOffset.WithTimeZone(this.timeZone.Local).ToString(this.Format, culture);
        }

        return UnsetValue;
    }

    protected override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value != null)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset;
            }

            if (value is string str)
            {
                if (DateTimeOffset.TryParse(str, out var parsedDateTimeOffset))
                {
                    return parsedDateTimeOffset.WithTimeZone(this.timeZone.Utc);
                }
            }
        }

        return UnsetValue;
    }
}
