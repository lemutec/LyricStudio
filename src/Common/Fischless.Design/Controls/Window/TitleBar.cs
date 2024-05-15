using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Fischless.Design.Controls;

[Obsolete("Not supported nowaday")]
public class TitleBar : TemplatedControl
{
    public static readonly AvaloniaProperty IconProperty = AvaloniaProperty.Register<TitleBar, IconElement>("Icon", null);

    public IconElement? Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly AvaloniaProperty TitleProperty = AvaloniaProperty.Register<TitleBar, string>("Title", null);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly AvaloniaProperty HeaderProperty = AvaloniaProperty.Register<TitleBar, object>("Header", null);

    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
}
