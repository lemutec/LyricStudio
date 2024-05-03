namespace Fischless.Design.Converters;

public class VisibilityInverter : BoolToValueConverter<Visibility>
{
    public VisibilityInverter()
    {
        this.TrueValue = Visibility.Visible;
        this.FalseValue = Visibility.Collapsed;
        this.IsInverted = true;
    }
}
