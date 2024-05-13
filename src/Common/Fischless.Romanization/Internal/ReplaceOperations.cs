using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

namespace Fischless.Romanization.Internal;

internal static class ReplaceOperations
{
    /// <summary>
    /// Replaces all instances of <paramref name="oldValue"/> with <paramref name="newValue"/> in <paramref name="str"/>,
    /// keeping the general casing the same while paying attention to the context (characters surrounding the replacement).<br />
    /// Examples of casing:<br />
    /// <list type="bullet">
    /// <item><description><c>C</c> → <c>Dd</c></description></item>
    /// <item><description><c>c</c> → <c>dd</c></description></item>
    /// <item><description>ABCD<c>R</c>GHI → ABCD<c>EF</c>GHI</description></item>
    /// <item><description>abcd<c>R</c>ghi → abcd<c>Ef</c>ghi</description></item>
    /// <item><description>abcd<c>r</c>ghi → abcd<c>ef</c>ghi</description></item>
    /// <item><description><c>AA</c> → <c>BBB</c></description></item>
    /// <item><description><c>Aa</c> → <c>Bbb</c></description></item>
    /// <item><description><c>aA</c> → <c>bbB</c></description></item>
    /// <item><description><c>aa</c> → <c>bbb</c></description></item>
    /// </list>
    /// NOTE: This function does all comparison and searching with the <see cref="CultureInfo.CurrentCulture"/> of <see cref="CultureInfo"/>.
    /// </summary>
    /// <param name="str">The string to replace in.</param>
    /// <param name="oldValue">The value to search for.</param>
    /// <param name="newValue">The value to replace with.</param>
    /// <param name="strComp">The string comparison type. Defaults to <see cref="StringComparison.CurrentCultureIgnoreCase"/>.</param>
    /// <returns>A string with all instances of <paramref name="oldValue"/> replaced with <paramref name="newValue"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="str"/> or <paramref name="oldValue"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="oldValue"/> is of length <c>0</c>.</exception>
    /// <remarks>The original code for this is from https://stackoverflow.com/a/45756981 </remarks>
    [Pure]
    public static string ReplaceWithSameCase(this string str, string oldValue, string newValue, StringComparison strComp = StringComparison.CurrentCultureIgnoreCase)
    {
        // Check inputs.
        if (str == null)
            throw new ArgumentNullException(nameof(str));
        if (str.Length == 0)
            return str;
        if (oldValue == null)
            throw new ArgumentNullException(nameof(oldValue));
        if (oldValue.Length == 0)
            throw new ArgumentException("String cannot be of zero length.");

        // Prepare string builder for storing the processed string.
        StringBuilder resultStringBuilder = new StringBuilder(str.Length);

        // Analyze the replacement: replace or remove.
        bool isReplacementNullOrEmpty = string.IsNullOrEmpty(newValue);

        // Replace all values.
        const int valueNotFound = -1;
        int foundAt;
        int startSearchFromIndex = 0;
        while ((foundAt = str.IndexOf(oldValue, startSearchFromIndex, strComp)) != valueNotFound)
        {
            // Append all characters until the found replacement.
            int charsUntilReplacement = foundAt - startSearchFromIndex;
            if (charsUntilReplacement != 0)
                resultStringBuilder.Append(str, startSearchFromIndex, charsUntilReplacement);

            // Process the replacement.
            if (!isReplacementNullOrEmpty)
            {
                // The logic for this is a bit messy, but the additional checks in lastCharUpper are to determine if the replacee should be counted as all-caps
                // This is to cover edge cases where str is something like ABCDRGHI, where R would otherwise be replaced with Ef instead of EF
                // TODO: Potentially add support for checks past things like vowel ties (k͡s) - this will likely involve splitting into surrogate pairs, which may be expensive
                bool firstCharUpper = char.IsUpper(str[foundAt]);
                if (newValue.Length > 1)
                {
                    bool lastCharUpper = char.IsUpper(str[foundAt + oldValue.Length - 1]) &&
                                         (oldValue.Length > 1 ||
                                          (str.Length > foundAt + 1 && char.IsUpper(str[foundAt + 1])) ||
                                          (foundAt > 0 && char.IsUpper(str[foundAt - 1])));
                    // example
                    if (!firstCharUpper && !lastCharUpper)
                        resultStringBuilder.Append(newValue.ToLower(CultureInfo.CurrentCulture));
                    // EXAMPLE
                    else if (firstCharUpper && lastCharUpper)
                        resultStringBuilder.Append(newValue.ToUpper(CultureInfo.CurrentCulture));
                    // Example
                    else if (firstCharUpper && !lastCharUpper)
                    {
                        resultStringBuilder.Append(char.ToUpper(newValue[0], CultureInfo.CurrentCulture));
                        resultStringBuilder.Append(newValue.Substring(1).ToLower(CultureInfo.CurrentCulture));
                    }
                    // examplE
                    else
                    {
                        resultStringBuilder.Append(newValue.Substring(0, newValue.Length - 1)
                            .ToLower(CultureInfo.CurrentCulture));
                        resultStringBuilder.Append(char.ToUpper(newValue[^1], CultureInfo.CurrentCulture));
                    }
                }
                else
                    resultStringBuilder.Append(firstCharUpper
                        ? newValue.ToUpper(CultureInfo.CurrentCulture)
                        : newValue.ToLower(CultureInfo.CurrentCulture));
            }

            // Prepare start index for the next search.
            // This needed to prevent infinite loop, otherwise method always start search
            // from the start of the string. For example: if an oldValue == "EXAMPLE", newValue == "example"
            // and comparisonType == "any ignore case" will conquer to replacing:
            // "EXAMPLE" to "example" to "example" to "example" … infinite loop.
            startSearchFromIndex = foundAt + oldValue.Length;
            if (startSearchFromIndex == str.Length)
            {
                // It is end of the input string: no more space for the next search.
                // The input string ends with a value that has already been replaced.
                // Therefore, the string builder with the result is complete and no further action is required.
                return resultStringBuilder.ToString();
            }
        }

        // Append the last part to the result.
        int charsUntilStringEnd = str.Length - startSearchFromIndex;
        resultStringBuilder.Append(str, startSearchFromIndex, charsUntilStringEnd);

        return resultStringBuilder.ToString();
    }

    [Pure]
    public static string ReplaceMany(this string text, params ISub[] subs)
        => subs.Aggregate(text, (str, sub) => sub.Replace(str));

    [Pure]
    public static string ReplaceFromChart(this string text, Dictionary<string, string> chart, StringComparison strComp = StringComparison.CurrentCulture)
        => chart.Keys.Aggregate(text, (current, key)
            => current.Replace(key, chart[key], strComp));

    [Pure]
    public static string ReplaceFromChartWithSameCase(this string text, Dictionary<string, string> chart, StringComparison strComp = StringComparison.CurrentCultureIgnoreCase)
        => chart.Keys.Aggregate(text, (current, key)
            => current.ReplaceWithSameCase(key, chart[key], strComp));

    // TODO: This needs to be removed and replaced with less assumptive methods
    /// <summary>
    /// Remove common alternative characters, such as the ideographic full-stop (replaced with a period).
    /// </summary>
    /// <param name="text">The text to replace in.</param>
    /// <returns>The original text with common alternate characters replaced.</returns>
    [Pure]
    internal static string ReplaceCommonAlternates(this string text)
        => text.Replace(Constants.IdeographicFullStop, '.')
            .Replace(Constants.Interpunct, ' ')
            .Replace(Constants.FullWidthSpace, ' ');

    [Pure]
    internal static string WithoutChars(this string charset, string withoutChars, StringComparison strComp = StringComparison.Ordinal)
        => withoutChars.Aggregate(charset, (set, withoutChar) => set.Replace($"{withoutChar}", "", strComp));

    [Pure]
    internal static string ReplaceMultipleChars(this string str, IEnumerable<char> chars, char replacement)
    {
        StringBuilder result = new(str.Length);
        foreach (char s in str)
        {
            bool replaced = false;
            foreach (char c in chars)
                if (s == c)
                {
                    result.Append(replacement);
                    replaced = true;
                    break;
                }
            if (!replaced)
                result.Append(s);
        }

        return result.ToString();
    }
}
