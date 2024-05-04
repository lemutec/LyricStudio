﻿using System.Globalization;

namespace Fischless.Design.Converters;

/// <summary>
/// EnumToBoolConverter can be used to bind to RadioButtons.
/// </summary>
// Source: http://stackoverflow.com/questions/397556/how-to-bind-radiobuttons-to-an-enum
public class EnumToBoolConverter : SingletonConverterBase<EnumToBoolConverter>
{
    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is string parameterString)
        {
            if (Enum.IsDefined(value.GetType(), value) == false)
            {
                return UnsetValue;
            }

            var parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        return UnsetValue;
    }

    protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is string parameterString)
        {
            return Enum.Parse(targetType, parameterString);
        }

        return UnsetValue;
    }
}
