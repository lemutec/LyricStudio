using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ude;

namespace LyricStudio.Core.LyricTrack;

public static partial class LrcHelper
{
    public static HashSet<string> LyricExtensions { get; } = [".lrc", ".krc", ".txt", ".ass"];

    /// <summary>
    /// Clear all Time Mark [00:00.000]
    /// </summary>
    [GeneratedRegex(@"\[\d+:\d+.\d+\]")]
    public static partial Regex StripTimeMarkRegex();

    /// <summary>
    /// Split lines
    /// </summary>
    [GeneratedRegex(@"\r?\n")]
    public static partial Regex SplitLineRegex();

    /// <summary>
    /// Such as [00:00.000]
    /// </summary>
    [GeneratedRegex(@"\[\d+\:\d+\.\d+\]")]
    public static partial Regex TimeMarkRegex();

    /// <summary>
    /// Such as [al:album]
    /// </summary>
    [GeneratedRegex(@"\[\w+\:.*\]")]
    public static partial Regex LrcInfoRegex();

    /// <summary>
    /// Search for pure lyrics text
    /// </summary>
    [GeneratedRegex(@"(?<=\])[^\]]+$")]
    public static partial Regex LyricRegex();

    public static string ReadAllText(string path)
    {
        using FileStream fileStream = File.OpenRead(path);
        using MemoryStream fileMemoryStream = new();
        fileStream.CopyTo(fileMemoryStream);

        CharsetDetector detector = new();
        using MemoryStream memoryStream = new(fileMemoryStream.ToArray());
        detector.Feed(memoryStream);
        detector.DataEnd();

        string detectedCharset = detector.Charset;
        Encoding encoding = string.IsNullOrWhiteSpace(detectedCharset) ? Encoding.UTF8 : Encoding.GetEncoding(detectedCharset);
        return File.ReadAllText(path, encoding);
    }

    public static IEnumerable<LrcLine> ParseText(string text)
    {
        List<LrcLine> lrcList = [];

        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        string[] lines = SplitLineRegex().Split(text);

        // The text does not contain timecode
        if (!TimeMarkRegex().IsMatch(text))
        {
            foreach (string line in lines)
            {
                if (LrcInfoRegex().IsMatch(line))
                {
                    lrcList.Add(new LrcLine(null, line.Trim('[', ']')));
                }
                else
                {
                    lrcList.Add(new LrcLine(0, line));
                }
            }
        }
        // The text contain timecode
        else
        {
            bool multiLrc = false;

            int lineNumber = 1;
            try
            {
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        lineNumber++;
                        continue;
                    }

                    MatchCollection matches = TimeMarkRegex().Matches(line);

                    // Such as [00:00.000][00:01.000]
                    if (matches.Count > 1)
                    {
                        string lrc = LyricRegex().Match(line).ToString();

                        lrcList.AddRange(matches.Select(match => new LrcLine(ParseTimeSpan(match.ToString().Trim('[', ']')), lrc)));
                        multiLrc = true;
                    }
                    // Normal line like [00:00.000]
                    else if (matches.Count == 1)
                    {
                        lrcList.Add(LrcLine.Parse(line));
                    }
                    // Info line
                    else if (LrcInfoRegex().IsMatch(line))
                    {
                        lrcList.Add(new LrcLine(null, LrcInfoRegex().Match(line).ToString().Trim('[', ']')));
                    }
                    // Not an empty line but no any timecode was found, so add an empty timecode
                    else
                    {
                        lrcList.Add(new LrcLine(TimeSpan.Zero, line));
                    }
                    lineNumber++;
                }
                // Multi timecode and sort it auto
                if (multiLrc)
                {
                    lrcList = [.. lrcList.OrderBy(x => x.LrcTime)];
                }
            }
            catch (Exception e)
            {
                // Some error occurred in {{ lineNumber }}
                Debug.WriteLine(e);
            }
        }

        return lrcList;
    }

    /// <summary>
    /// Resolves the timestamp string to a TimeSpan
    /// </summary>
    public static TimeSpan ParseTimeSpan(string s)
    {
        // If the millisecond is two-digit, add an extra 0 at the end
        if (s.Split('.')[1].Length == 2)
        {
            s += '0';
        }
        return TimeSpan.Parse("00:" + s);
    }

    /// <summary>
    /// Try to resolve the timestamp string to TimeSpan, see
    /// <seealso cref="ParseTimeSpan(string)"/>
    /// </summary>
    public static bool TryParseTimeSpan(string s, out TimeSpan ts)
    {
        try
        {
            ts = ParseTimeSpan(s);
            return true;
        }
        catch
        {
            ts = TimeSpan.Zero;
            return false;
        }
    }

    /// <summary>
    /// Change the timestamp to a two-digit millisecond format
    /// </summary>
    public static string ToShortString(this TimeSpan ts, bool isShort = false)
    {
        if (isShort)
        {
            return $"{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
        }
        else
        {
            return $"{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
        }
    }

    public static LrcLine GetNearestLrc(IEnumerable<LrcLine> lrcList, TimeSpan time)
    {
        LrcLine? line = lrcList
            .Where(x => x.LrcTime != null && x.LrcTime <= time)
            .OrderByDescending(x => x.LrcTime)
            .FirstOrDefault();

        return line;
    }

    public static string StripTimeMark(string lrc)
    {
        return StripTimeMarkRegex().Replace(lrc, string.Empty);
    }
}
