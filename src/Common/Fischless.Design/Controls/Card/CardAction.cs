using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Metadata;

namespace Fischless.Design.Controls;

[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
public class CardAction : ContentControl
{
    public new static readonly StyledProperty<object?> ContentProperty = AvaloniaProperty.Register<CardAction, object?>("Content");

    [Content]
    [DependsOn("ContentTemplate")]
    public new object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
}
