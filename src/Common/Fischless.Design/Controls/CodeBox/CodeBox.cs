using Avalonia;
using AvaloniaEdit;

namespace Fischless.Design.Controls;

[Obsolete("Not Impl")]
public class CodeBox : TextEditor
{
    public string Code
    {
        get => Text;
        set => Text = value;
    }

    public static readonly AvaloniaProperty CodeProperty =
        AvaloniaProperty.Register<CodeBox, string>(nameof(Code), string.Empty, coerce: OnCodeCoerce);

    private static string OnCodeCoerce(AvaloniaObject d, string value)
    {
        if (d is TextEditor editor)
        {
            editor.Text = value;
        }
        return value;
    }
}
