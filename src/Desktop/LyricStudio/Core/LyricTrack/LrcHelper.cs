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
    [GeneratedRegex(@"\r?\n")]
    public static partial Regex SplitLineRegex();

    /// <summary>
    /// Suck as [00:00.000]
    /// </summary>
    [GeneratedRegex(@"\[\d+\:\d+\.\d+\]")]
    public static partial Regex TimeMarkRegex();

    /// <summary>
    /// Suck as [al:album]
    /// </summary>
    [GeneratedRegex(@"\[\w+\:.+\]")]
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
        return File.ReadAllText(path, Encoding.GetEncoding(detectedCharset));
    }

    public static IEnumerable<LrcLine> ParseText(string text)
    {
        List<LrcLine> lrcList = [];

        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        string[] lines = SplitLineRegex().Split(text);

        // 文本中不包含时间信息
        if (!TimeMarkRegex().IsMatch(text))
        {
            foreach (var line in lines)
            {
                // 即便是不包含时间信息的歌词文本，也可能出现歌词信息
                if (LrcInfoRegex().IsMatch(line))
                {
                    lrcList.Add(new LrcLine(null, line.Trim('[', ']')));
                }
                // 否则将会为当前歌词行添加空白的时间标记，即便当前行是空行
                else
                    lrcList.Add(new LrcLine(0, line));
            }
        }
        // 文本中包含时间信息
        else
        {
            // 如果在解析过程中发现存在单行的多时间标记的情况，会在最后进行排序
            bool multiLrc = false;

            int lineNumber = 1;
            try
            {
                foreach (var line in lines)
                {
                    // 在确认文本中包含时间标记的情况下，会忽略所有空行
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        lineNumber++;
                        continue;
                    }

                    var matches = TimeMarkRegex().Matches(line);
                    // 出现了类似 [00:00.000][00:01.000] 的包含多个时间信息的歌词行
                    if (matches.Count > 1)
                    {
                        var lrc = LyricRegex().Match(line).ToString();
                        foreach (var match in matches)
                        {
                            lrcList.Add(
                                new LrcLine(
                                    LrcHelper.ParseTimeSpan(match.ToString().Trim('[', ']')),
                                    lrc
                                )
                            );
                        }

                        multiLrc = true;
                    }
                    // 常规的单行歌词 [00:00.000]
                    else if (matches.Count == 1)
                    {
                        lrcList.Add(LrcLine.Parse(line));
                    }
                    // 说明这是一个歌词信息行
                    else if (LrcInfoRegex().IsMatch(line))
                    {
                        lrcList.Add(
                            new LrcLine(null, LrcInfoRegex().Match(line).ToString().Trim('[', ']'))
                        );
                    }
                    // 说明正常的歌词里面出现了一个不是空行，却没有时间标记的内容，则添加空时间标记
                    else
                    {
                        lrcList.Add(new LrcLine(TimeSpan.Zero, line));
                    }
                    lineNumber++;
                }
                // 如果出现单行出现多个歌词信息的情况，所以进行排序
                if (multiLrc)
                    lrcList = [.. lrcList.OrderBy(x => x.LrcTime)];
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
}
