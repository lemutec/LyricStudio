using System.Text;

namespace Fischless.Romanization;

public static class KanjiConverter
{
    public static async Task<string?> Convert(string text)
    {
        StringBuilder sb = new();
        KawazuConverter converter = new();

        foreach (char ch in text)
        {
            string? result = await converter.Convert(ch.ToString(), To.Katakana, Mode.Normal, RomajiSystem.Nippon, "(", ")");

            if (string.IsNullOrEmpty(result))
            {
                string textTraditional = ChineseConverter.ToTraditional(ch.ToString());
                sb.Append(textTraditional);
            }
            else
            {
                sb.Append(ch);
            }
        }

        return sb.ToString();
    }
}
