using Avalonia;

namespace Fischless.Design.Converters;

public class EnumWrapperConverter : EnumWrapperConverterBase<EnumWrapperConverter>
{
    public static readonly AvaloniaProperty NameStyleProperty = AvaloniaProperty.Register<EnumWrapperConverter, EnumWrapperConverterNameStyle>(
        "NameStyle",
        EnumWrapperConverterNameStyle.LongName);

    public override EnumWrapperConverterNameStyle NameStyle
    {
        get => (EnumWrapperConverterNameStyle)this.GetValue(NameStyleProperty);
        set => this.SetValue(NameStyleProperty, value);
    }
}
