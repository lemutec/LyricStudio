using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Vanara.InteropServices;
using Vanara.PInvoke;

namespace Fischless.Win32.Natives;

[SupportedOSPlatform("Windows")]
public static class User32
{
    [SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.")]
    public static string GetString(DialogBoxCommand wBtn)
    {
        StrPtrUni strPtrUni = Vanara.PInvoke.User32.MB_GetString((uint)wBtn);
        string src = strPtrUni.ToString()?.TrimStart('&')!;
        return new Regex(@"\([^)]*\)").Replace(src, string.Empty).Replace("&", string.Empty);
    }

    /// <summary>
    /// Represents possible dialogbox command id values by the MB_GetString function.
    /// </summary>
    [PInvokeData("Winuser.h")]
    [Flags]
    public enum DialogBoxCommand : int
    {
        IDOK = 0,
        IDCANCEL = 1,
        IDABORT = 2,
        IDRETRY = 3,
        IDIGNORE = 4,
        IDYES = 5,
        IDNO = 6,
        IDCLOSE = 7,
        IDHELP = 8,
        IDTRYAGAIN = 9,
        IDCONTINUE = 10,
    }
}
