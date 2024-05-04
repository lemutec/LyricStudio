using MediaInfoLib;
using System.Runtime.Versioning;

namespace Fischless.Win32;

[SupportedOSPlatform("Windows")]
public static class MediaInfoProvider
{
    public static string Inform(string fileName)
    {
        using MediaInfo mi = new();
        mi.Open(fileName);
        string inform = mi.Inform();
        mi.Close();
        return inform;
    }
}
