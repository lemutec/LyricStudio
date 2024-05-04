using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System.Reflection;
using System.Runtime.Versioning;

namespace Fischless.Win32.SystemDialog;

/// <summary>
/// <see cref="Avalonia.Controls.SaveFileDialog"/>
/// </summary>
[SupportedOSPlatform("Windows")]
[SupportedOSPlatform("MacOS")]
public class SaveFileDialog
{
    /// <summary>
    /// Gets or sets the dialog title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the default extension to be used to save the file (including the period ".").
    /// </summary>
    public string? DefaultExtension { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display a warning if the user specifies the name of a file that already exists.
    /// </summary>
    public bool? ShowOverwritePrompt { get; set; }

    /// <summary>
    /// Gets or sets a collection of filters which determine the types of files displayed in an
    /// <see cref="OpenFileDialog"/> or an <see cref="SaveFileDialog"/>.
    /// </summary>
    public List<FileDialogFilter> Filters { get; set; } = [];

    /// <summary>
    /// Gets or sets initial file name that is displayed when the dialog is opened.
    /// </summary>
    public string? InitialFileName { get; set; }

    /// <summary>
    /// Gets or sets the initial directory that will be displayed when the file system dialog
    /// is opened.
    /// </summary>
    public string? Directory { get; set; }

    /// <summary>
    /// Shows the save file dialog.
    /// </summary>
    /// <param name="parent">The parent window.</param>
    /// <returns>
    /// A task that on completion contains the full path of the save location, or null if the
    /// dialog was canceled.
    /// </returns>
    public async Task<SaveFileDialogResult?> ShowAsync(TopLevel parent = null)
    {
        parent ??= (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow ?? new Window();

        IStorageFile? storageFile = await parent?.StorageProvider.SaveFilePickerAsync(ToFilePickerSaveOptions());

        if (storageFile == null)
        {
            return null;
        }

        if (storageFile?.GetType().GetProperty(nameof(FileInfo)) is PropertyInfo pi)
        {
            return new SaveFileDialogResult()
            {
                File = storageFile,
                FileInfo = (pi.GetValue(storageFile) as FileInfo)
            };
        }

        return new SaveFileDialogResult()
        {
            File = storageFile,
            FileInfo = null
        };
    }

    public FilePickerSaveOptions ToFilePickerSaveOptions()
    {
        return new FilePickerSaveOptions
        {
            SuggestedFileName = InitialFileName,
            DefaultExtension = DefaultExtension,
            FileTypeChoices = Filters?.Select(f => new FilePickerFileType(f.Name!) { Patterns = f.Extensions.Select(e => $"*.{e}").ToArray() }).ToArray(),
            Title = Title,
            SuggestedStartLocation = Directory is { } directory
                ? typeof(IStorageFile).Assembly.GetType("Avalonia.Platform.Storage.FileIO.BclStorageFolder").GetConstructor([typeof(DirectoryInfo)]).Invoke([directory]) as IStorageFolder
                : null,
            ShowOverwritePrompt = ShowOverwritePrompt
        };
    }
}

public class SaveFileDialogResult : OpenFileDialogResultItem
{
}
