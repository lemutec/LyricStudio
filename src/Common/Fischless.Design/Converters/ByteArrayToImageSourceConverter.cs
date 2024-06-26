﻿using Avalonia.Media.Imaging;
using System.Globalization;

namespace Fischless.Design.Converters;

public sealed class ByteArrayToImageSourceConverter : SingletonConverterBase<ByteArrayToImageSourceConverter>
{
    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        int? width = null!;

        if (parameter is string widthString)
        {
            if (int.TryParse(widthString, out int widthValue))
            {
                width = widthValue;
            }
        }

        if (value is byte[] bytes)
        {
            using MemoryStream stream = new(bytes);

            if (width != null)
            {
                return Bitmap.DecodeToWidth(stream, width.Value);
            }
            else
            {
                return new Bitmap(stream);
            }
        }

        return null;
    }
}
