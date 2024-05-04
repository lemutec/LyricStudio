﻿using System.Diagnostics;
using System.Globalization;

namespace Fischless.Design.Converters;

public class DebugConverter : SingletonConverterBase<DebugConverter>
{
    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        Debug.WriteLine("DebugConverter.Convert(value={0}, targetType={1}, parameter={2}, culture={3}",
            value ?? "null",
            (object)targetType ?? "null",
            parameter ?? "null",
            (object)culture ?? "null");

        return value;
    }

    protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        Debug.WriteLine("DebugConverter.ConvertBack(value={0}, targetType={1}, parameter={2}, culture={3}",
             value ?? "null",
             (object)targetType ?? "null",
             parameter ?? "null",
             (object)culture ?? "null");

        return value;
    }
}
