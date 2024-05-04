using MediaInfoLib;
using System.Runtime.Versioning;

namespace Fischless.Win32;

[SupportedOSPlatform("Windows")]
public static class MediaInfoAudio
{
    public static bool HasAudioTrack(string fileName)
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            using MediaInfo lib = new();
            return lib.WithOpen(fileName).Count_Get(StreamKind.Audio) > 0;
        }
        return false;
    }

    public static double GetAudioBitRate(string fileName)
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            using MediaInfo lib = new();
            lib.Open(fileName);
            _ = double.TryParse(lib.Get(StreamKind.Audio, 0, "BitRate"), out double bitRate);
            lib.Close();
            return bitRate;
        }
        return default;
    }
}
