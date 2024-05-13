using System;
using System.Diagnostics;

namespace LyricStudio.Core.LyricTrack;

/// <summary>
/// https://en.wikipedia.org/wiki/LRC_(file_format)
/// </summary>
[DebuggerDisplay("{PreviewText}")]
public class LrcLine : IComparable<LrcLine>
{
    public static readonly LrcLine Empty = new();

    public TimeSpan? LrcTime { get; set; } = default;

    public static bool IsShort { get; set; } = false;

    public string LrcTimeText
    {
        get => LrcTime.HasValue ? LrcHelper.ToShortString(LrcTime.Value, IsShort) : string.Empty;
        set
        {
            if (LrcHelper.TryParseTimeSpan(value, out TimeSpan ts))
            {
                LrcTime = ts;
            }
            else
            {
                LrcTime = null;
            }
        }
    }

    public string LrcText { get; set; }

    /// <summary>
    /// Preview such as [{LrcTime:mm:ss.fff}]{LyricText}
    /// </summary>
    public string PreviewText
    {
        get
        {
            if (LrcTime.HasValue)
            {
                return $"[{LrcHelper.ToShortString(LrcTime.Value, IsShort)}]{LrcText}";
            }
            else if (!string.IsNullOrWhiteSpace(LrcText))
            {
                return $"[{LrcText}]";
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public LrcLine(double time, string text)
    {
        LrcTime = new TimeSpan(0, 0, 0, 0, (int)(time * 1000));
        LrcText = text;
    }

    public LrcLine(TimeSpan? time, string text)
    {
        LrcTime = time;
        LrcText = text;
    }

    public LrcLine(TimeSpan? time)
        : this(time, string.Empty)
    {
    }

    public LrcLine(LrcLine lrcLine)
    {
        LrcTime = lrcLine.LrcTime;
        LrcText = lrcLine.LrcText;
    }

    public LrcLine(string line)
        : this(Parse(line))
    {
    }

    public LrcLine()
    {
        LrcTime = null;
        LrcText = string.Empty;
    }

    public static LrcLine Parse(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return Empty;
        }

        if (CheckMultiLine(line))
        {
            throw new FormatException();
        }

        string[]? slices = line.TrimStart().TrimStart('[').Split(']');

        if (!LrcHelper.TryParseTimeSpan(slices[0], out TimeSpan time))
        {
            return new LrcLine(null, slices[0]);
        }

        return new LrcLine(time, slices[1]);
    }

    public static bool TryParse(string line, out LrcLine lrcLine)
    {
        try
        {
            lrcLine = Parse(line);
            return true;
        }
        catch
        {
            lrcLine = Empty;
            return false;
        }
    }

    public static bool CheckMultiLine(string line)
    {
        if (line.TrimStart().IndexOf('[', 1) != -1) return true;
        else return false;
    }

    public override string ToString() => PreviewText;

    public int CompareTo(LrcLine? other)
    {
        // Sort order: null < TimeSpan < string
        if (!LrcTime.HasValue) return -1;
        if (!other.LrcTime.HasValue) return 1;
        return LrcTime.Value.CompareTo(other.LrcTime.Value);
    }
}
