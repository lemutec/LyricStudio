using Avalonia.Media;

namespace Fischless.Design.Converters;

public class BoolToFontWeightConverter : BoolToValueConverter<FontWeight>
{
    public BoolToFontWeightConverter()
    {
        this.TrueValue = FontWeight.Bold;
        this.FalseValue = FontWeight.Normal;
    }
}
