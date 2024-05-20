using Fischless.Win32.Natives;

namespace Fischless.Design.Controls;

public class MessageBoxButtonText
{
    public string Ok { get; } = GetString(MessageBoxResult.Ok);
    public string Yes { get; } = GetString(MessageBoxResult.Yes);
    public string No { get; } = GetString(MessageBoxResult.No);
    public string Abort { get; } = GetString(MessageBoxResult.Abort);
    public string Cancel { get; } = GetString(MessageBoxResult.Cancel);

    private static string GetString(MessageBoxResult button)
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            return User32.GetString(button switch
            {
                MessageBoxResult.Ok => User32.DialogBoxCommand.IDOK,
                MessageBoxResult.Yes => User32.DialogBoxCommand.IDYES,
                MessageBoxResult.No => User32.DialogBoxCommand.IDNO,
                MessageBoxResult.Abort => User32.DialogBoxCommand.IDABORT,
                MessageBoxResult.Cancel => User32.DialogBoxCommand.IDCANCEL,
                _ => User32.DialogBoxCommand.IDIGNORE,
            });
        }

        return button.ToString();
    }
}
