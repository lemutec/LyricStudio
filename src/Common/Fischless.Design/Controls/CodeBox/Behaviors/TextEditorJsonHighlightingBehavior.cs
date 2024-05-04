using Avalonia.Xaml.Interactivity;
using AvaloniaEdit;

namespace Fischless.Design.Controls;

public sealed class TextEditorJsonHighlightingBehavior : Behavior<TextEditor>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.RegisterJson();
    }
}
