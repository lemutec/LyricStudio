using Fischless.Design.Converters;
using System;
using System.Globalization;

namespace LyricStudio.Views.Converters;

public sealed class SecondsToTimeStringConverter : SingletonConverterBase<SecondsToTimeStringConverter>
{
    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            return $"{timeSpan:mm\\:ss}";
        }

        return null;
    }
}
