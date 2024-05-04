namespace Fischless.Win32.SystemDialog;

public class FileDialogFilter
{
    /// <summary>
    /// Gets or sets the name of the filter, e.g. ("Text files (.txt)").
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a list of file extensions matched by the filter (e.g. "txt" or "*" for all
    /// files).
    /// </summary>
    public List<string> Extensions { get; set; } = [];
}
