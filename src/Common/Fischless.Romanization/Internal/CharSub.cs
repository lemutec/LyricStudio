using System.Text.RegularExpressions;

namespace Fischless.Romanization.Internal;

internal class CharSub : ISub
{
    private readonly Regex _findRegex;
    private readonly string _substitution;

    public CharSub(string pattern, string substitution, bool ignoreCase = true)
    {
        _findRegex = new Regex(pattern, ignoreCase ? RegexOptions.Compiled | RegexOptions.IgnoreCase : RegexOptions.Compiled);
        _substitution = substitution;
    }

    public CharSub(string pattern, string substitution, RegexOptions options)
    {
        _findRegex = new Regex(pattern, RegexOptions.Compiled | options);
        _substitution = substitution;
    }

    public string Replace(string text)
        => _findRegex.Replace(text, _substitution);
}
