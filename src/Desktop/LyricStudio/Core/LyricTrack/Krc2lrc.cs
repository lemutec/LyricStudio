using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LyricStudio.Core.LyricTrack;

public sealed partial class Krc2lrc
{
    /// <summary>
    /// <see raw="@Gaw^2tGQ61-ÎÒni"/>
    /// </summary>
    public static readonly byte[] DecryptKey = [0x40, 0x47, 0x61, 0x77, 0x5e, 0x32, 0x74, 0x47, 0x51, 0x36, 0x31, 0x2d, 0xce, 0xd2, 0x6e, 0x69];

    [GeneratedRegex(@"\r?\n")]
    private static partial Regex SplitLineRegex();

    [GeneratedRegex(@"\[(\d+),(\d+)\]")]
    private static partial Regex KTimeCodeRegex();

    [GeneratedRegex(@"<\d+,\d+,\d+>")]
    private static partial Regex KTimeRegex();

    public static bool KrcToLyric(byte[] raw, out string lrc)
    {
        try
        {
            if (!raw[..4].SequenceEqual(Encoding.UTF8.GetBytes("krc1")))
            {
                throw new MethodAccessException("Error file format.");
            }

            byte[] data = raw[4..];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ DecryptKey[i % 16]);
            }

            if (!Decompress(data, out byte[] dst))
            {
                throw new MethodAccessException("Decompression failed.");
            }

            string krc = Encoding.UTF8.GetString(dst);
            lrc = SwapTimeCode(krc[1..]);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        lrc = null!;
        return false;
    }

    private static bool Decompress(byte[] src, out byte[] dst)
    {
        try
        {
            using MemoryStream inputStream = new(src, 0, src.Length);
            using MemoryStream outputStream = new();
            using InflaterInputStream decompressionStream = new(inputStream);

            decompressionStream.CopyTo(outputStream);
            dst = outputStream.ToArray();
            return true;
        }
        catch
        {
            dst = null;
            return false;
        }
    }

    private static string SwapTimeCode(string lrc)
    {
        StringBuilder sb = new();
        string[] lines = SplitLineRegex().Split(lrc);

        foreach (string line in lines)
        {
            Match match = KTimeCodeRegex().Match(line);

            if (match.Success)
            {
                double startTime = double.Parse(match.Groups[1].Value);
                double duration = double.Parse(match.Groups[2].Value);
                string text = line.Replace(KTimeCodeRegex(), string.Empty)
                    .Replace(KTimeRegex(), string.Empty);
                LrcLine lrcLine = new(TimeSpan.FromMilliseconds(startTime), text);

                _ = duration;
                sb.AppendLine(lrcLine.ToString());
            }
            else
            {
                sb.AppendLine(line);
            }
        }
        return sb.ToString();
    }
}

file static class StringExtension
{
    public static string Replace(this string input, Regex regex, string replacement)
    {
        return regex.Replace(input, replacement);
    }
}
