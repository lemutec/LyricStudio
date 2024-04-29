using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace LyricStudio.Views.Converters;

public sealed class SecondsToTimeStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            return $"{timeSpan:mm\\:ss}";
        }

        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
