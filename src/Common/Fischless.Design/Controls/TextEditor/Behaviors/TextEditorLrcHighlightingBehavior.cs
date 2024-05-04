using Avalonia.Xaml.Interactivity;
using AvaloniaEdit;

namespace Fischless.Design.Controls;

public sealed class TextEditorLrcHighlightingBehavior : Behavior<TextEditor>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.RegisterLrc();
    }
}
