namespace Fischless.Romanization;

public static class RomanConverter
{
    public static async Task<string?> Convert(string text, RomanizationMode mode = RomanizationMode.Romaji)
    {
        KawazuConverter converter = new();

        if (mode == RomanizationMode.Romaji)
        {
            string? result1 = await converter.Convert(text, To.Katakana, Mode.Spaced, RomajiSystem.Nippon, "(", ")");
            string? result2 = new Japanese.ModifiedHepburn().Process(result1);
            return result2;
        }
        else if (mode == RomanizationMode.Hiragana)
        {
            string? result = await converter.Convert(text, To.Hiragana, Mode.Normal, RomajiSystem.Nippon, "(", ")");
            return result;
        }
        else if (mode == RomanizationMode.Katakana)
        {
            string? result = await converter.Convert(text, To.Katakana, Mode.Normal, RomajiSystem.Nippon, "(", ")");
            return result;
        }

        return null;
    }
}

public enum RomanizationMode
{
    Romaji,
    Hiragana,
    Katakana
}
