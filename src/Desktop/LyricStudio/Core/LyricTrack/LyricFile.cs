using System.IO;
using System.Text;
using Ude;

namespace LyricStudio.Core.LyricTrack;

public static class LyricFile
{
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
}
