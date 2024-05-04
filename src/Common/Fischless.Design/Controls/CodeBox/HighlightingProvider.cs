using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using Fischless.Design.Resources;
using System.Xml;

namespace Fischless.Design.Controls;

public static class HighlightingProvider
{
    public static void RegisterJson(this TextEditor self)
    {
        IHighlightingDefinition highlighting;
        using Stream s = ResourceLoader.GetStream("avares://Fischless.Design/Assets/Highlighting/Json.xshd");
        using XmlReader reader = new XmlTextReader(s);
        highlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

        HighlightingManager.Instance.RegisterHighlighting("Json", [".json"], highlighting);
        self.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Json");
    }

    public static void RegisterLrc(this TextEditor self)
    {
        IHighlightingDefinition highlighting;
        using Stream s = ResourceLoader.GetStream("avares://Fischless.Design/Assets/Highlighting/Lrc.xshd");
        using XmlReader reader = new XmlTextReader(s);
        highlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

        HighlightingManager.Instance.RegisterHighlighting("Lrc", [".lrc"], highlighting);
        self.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Lrc");
    }
}
