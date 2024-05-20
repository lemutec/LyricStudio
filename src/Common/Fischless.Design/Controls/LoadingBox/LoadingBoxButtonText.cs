using Fischless.Win32.Natives;

namespace Fischless.Design.Controls;

public class LoadingBoxButtonText
{
    public string Cancel { get; } = GetString(LoadingBoxResult.Cancel);

    private static string GetString(LoadingBoxResult button)
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            return User32.GetString(button switch
            {
                LoadingBoxResult.Cancel => User32.DialogBoxCommand.IDCANCEL,
                _ => User32.DialogBoxCommand.IDIGNORE,
            });
        }

        return button.ToString();
    }
}
