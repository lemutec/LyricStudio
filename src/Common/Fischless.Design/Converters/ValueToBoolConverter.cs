using Avalonia;

namespace Fischless.Design.Converters;

public class ValueToBoolConverter<T> : ReversibleValueToBoolConverterBase<T, ValueToBoolConverter<T>>
{
    public override T TrueValue
    {
        get => (T)this.GetValue(TrueValueProperty);
        set => this.SetValue(TrueValueProperty, value);
    }

    public static readonly AvaloniaProperty TrueValueProperty =
        PropertyHelper.Create<T, ValueToBoolConverter<T>>(nameof(TrueValue));

    public override T FalseValue
    {
        get => (T)this.GetValue(FalseValueProperty);
        set => this.SetValue(FalseValueProperty, value);
    }

    public static readonly AvaloniaProperty FalseValueProperty =
        PropertyHelper.Create<T, ValueToBoolConverter<T>>(nameof(FalseValue));
}

public class ValueToBoolConverter : ValueToBoolConverter<object>
{
}
