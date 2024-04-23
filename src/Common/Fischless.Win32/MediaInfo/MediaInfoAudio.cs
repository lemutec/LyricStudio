using MediaInfoLib;

namespace Fischless.Win32;

public static class MediaInfoAudio
{
    public static double GetAudioBitRate(string fileName)
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            MediaInfo lib = new();
            lib.Open(fileName);
            _ = double.TryParse(lib.Get(StreamKind.Audio, 0, "BitRate"), out double bitRate);
            lib.Close();
            return bitRate;
        }
        return default;
    }
}
