using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System.Collections;
using System.Reflection;
using System.Runtime.Versioning;

namespace Fischless.Win32.SystemDialog;

/// <summary>
/// https://github.com/AvaloniaUI/Avalonia/issues/9787
/// <see cref="Avalonia.Controls.OpenFileDialog"/>
/// </summary>
[SupportedOSPlatform("Windows")]
[SupportedOSPlatform("MacOS")]
public class OpenFileDialog
{
    /// <summary>
    /// Gets or sets the text that appears in the title bar of a picker.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user can select multiple files.
    /// </summary>
    public bool AllowMultiple { get; set; }

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
    /// Shows the open file dialog.
    /// </summary>
    /// <param name="parent">The parent window.</param>
    /// <returns>
    /// A task that on completion returns an array containing the full path to the selected
    /// files, or null if the dialog was canceled.
    /// </returns>
    public async Task<OpenFileDialogResult?> ShowAsync(TopLevel parent = null!)
    {
        parent ??= (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow ?? new Window();

        IReadOnlyList<IStorageFile> storageFile = await parent?.StorageProvider.OpenFilePickerAsync(ToFilePickerOpenOptions());

        if (storageFile == null || storageFile.Count <= 0)
        {
            return null;
        }

        return new OpenFileDialogResult()
        {
            Items = storageFile.Select((IStorageFile storageFile) =>
            {
                if (storageFile == null)
                {
                    return null;
                }

                if (storageFile.GetType().GetProperty(nameof(FileInfo)) is PropertyInfo pi)
                {
                    return new OpenFileDialogResultItem()
                    {
                        File = storageFile,
                        FileInfo = pi.GetValue(storageFile) as FileInfo,
                    };
                }

                return new OpenFileDialogResultItem()
                {
                    File = storageFile,
                    FileInfo = null,
                };
            })?.ToArray()!
        };
    }

    /// <summary>
    /// https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Base/Platform/Storage/FileIO/BclStorageFolder.cs
    /// <see cref="Avalonia.Platform.Storage.FileIO.BclStorageFolder"/>
    /// </summary>
    public FilePickerOpenOptions ToFilePickerOpenOptions()
    {
        return new FilePickerOpenOptions
        {
            AllowMultiple = AllowMultiple,
            FileTypeFilter = Filters?.Select(f => new FilePickerFileType(f.Name!) { Patterns = f.Extensions.Select(e => $"*.{e}").ToArray() }).ToArray(),
            Title = Title,
            SuggestedStartLocation = Directory is { } directory
                ? typeof(IStorageFile).Assembly.GetType("Avalonia.Platform.Storage.FileIO.BclStorageFolder").GetConstructor([typeof(DirectoryInfo)]).Invoke([directory]) as IStorageFolder
                : null
        };
    }
}

public class OpenFileDialogResult : IEnumerable, IEnumerable<OpenFileDialogResultItem>
{
    public OpenFileDialogResultItem[]? Items { get; internal set; } = [];
    public OpenFileDialogResultItem Item => Items?.FirstOrDefault();

    public IEnumerator<OpenFileDialogResultItem> GetEnumerator()
    {
        return ((IEnumerable<OpenFileDialogResultItem>)Items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Items.GetEnumerator();
    }
}

public class OpenFileDialogResultItem
{
    public IStorageFile File { get; internal set; } = default;

    public FileInfo FileInfo { get; internal set; } = default;
}
