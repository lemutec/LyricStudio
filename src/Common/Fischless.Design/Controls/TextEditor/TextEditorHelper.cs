using Avalonia;
using AvaloniaEdit;
using System.Diagnostics.CodeAnalysis;

namespace Fischless.Design.Controls;

public class TextEditorHelper
{
    public static readonly AttachedProperty<string> CodeProperty =
        AvaloniaProperty.RegisterAttached<TextEditorHelper, TextEditor, string>("Code", coerce: OnCodeCoerce);

    public static Dictionary<int, bool> TextChangedRegister { get; set; } = [];

    [SuppressMessage("Performance", "CA1864:Prefer the 'IDictionary.TryAdd(TKey, TValue)' method")]
    private static string OnCodeCoerce(AvaloniaObject d, string value)
    {
        if (d is TextEditor editor)
        {
            int editorHashCode = editor.GetHashCode();

            if (!TextChangedRegister.ContainsKey(editorHashCode))
            {
                TextChangedRegister.Add(editorHashCode, true);
                editor.TextChanged += OnTextChanged;
            }

            if (TextChangedRegister.TryGetValue(editorHashCode, out bool canUpdate) && canUpdate)
            {
                TextChangedRegister[editorHashCode] = false;
                editor.Text = value;
                TextChangedRegister[editorHashCode] = true;
            }
        }
        return value;
    }

    private static void OnTextChanged(object? sender, EventArgs e)
    {
        if (sender is TextEditor editor)
        {
            int editorHashCode = editor.GetHashCode();

            if (TextChangedRegister.TryGetValue(editorHashCode, out bool canUpdate) && canUpdate)
            {
                TextChangedRegister[editorHashCode] = false;
                SetCode(editor, editor.Text);
                TextChangedRegister[editorHashCode] = true;
            }
        }
    }

    public static string GetCode(TextEditor control)
        => control.GetValue(CodeProperty);

    public static void SetCode(TextEditor control, string value)
        => control.SetValue(CodeProperty, value);
}
