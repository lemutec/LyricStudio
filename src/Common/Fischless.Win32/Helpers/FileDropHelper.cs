using Avalonia.Input;
using System.Diagnostics.CodeAnalysis;

namespace Fischless.Win32.Helpers;

[SuppressMessage("Usage", "CS0618:Obsolete Method")]
[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
public static class FileDropHelper
{
    public static IEnumerable<string>? GetFileNames(this DragEventArgs e)
    {
#pragma warning disable CS0618
        if (e.Data.GetFileNames() is { } fileNames)
#pragma warning restore CS0618
        {
            return fileNames;
        }
        return null;
    }
}
