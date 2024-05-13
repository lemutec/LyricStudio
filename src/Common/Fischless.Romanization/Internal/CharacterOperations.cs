using System.Diagnostics.Contracts;
using System.Text;

namespace Fischless.Romanization.Internal;

internal static class CharacterOperations
{
    [Pure]
    public static string[] SplitIntoSurrogatePairs(this string str)
    {
        List<string> retList = new List<string>(str.Length);
        for (int i = 0; i < str.Length; i++)
        {
            if (!char.IsSurrogatePair(str, i))
            {
                retList.Add(str[i].ToString());
                continue;
            }

            retList.Add(str[i].ToString() + str[++i]);
        }

        return retList.ToArray();
    }

    [Pure]
    public static string UnicodeNormalize(this string str)
        => str.Normalize(NormalizationForm.FormD);

    private const string LanguageBoundaryChars = @"a-z";

    private static readonly Lazy<CharSub> LanguageBoundarySubstitution = new Lazy<CharSub>(() =>
        new CharSub(
            $"(?:([{LanguageBoundaryChars}{Constants.Punctuation}])([^ {LanguageBoundaryChars}{Constants.Punctuation}])|([^ {LanguageBoundaryChars}{Constants.Punctuation}])([{LanguageBoundaryChars}]))",
            "${1}${3} ${2}${4}"));

    /// <summary>
    /// Insert spaces at boundaries between Latin and non-Latin characters (ie. <c>ニンテンドーDSiブラウザー</c> -> <c>ニンテンドー DSi ブラウザー</c>).
    /// </summary>
    /// <param name="text">The text to insert spaces in.</param>
    /// <returns>The text with spaces inserted at language boundaries.</returns>
    [Pure]
    internal static string SeparateLanguageBoundaries(this string text)
        => LanguageBoundarySubstitution.Value.Replace(text);

    [Pure]
    internal static TResult? DetermineResultFromString<TResult>(this string text,
        params (TResult res, string[] set)[] possibleKinds)
        where TResult : struct
    {
        foreach ((TResult res, string[] set) possibleKind in possibleKinds)
            if (possibleKind.set.Any(text.Contains))
                return possibleKind.res;
        return null;
    }
}
