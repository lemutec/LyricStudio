using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using System.Threading.Tasks;

namespace LyricStudio.Services;

public class ClipboardService : IClipboardService
{
    private TopLevel topLevel = null!;

    private TopLevel GetTopLevel()
    {
        topLevel ??= (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow ?? new Window();
        return topLevel;
    }

    public async Task<string?> GetTextAsync()
    {
        return await GetTopLevel().Clipboard.GetTextAsync();
    }

    public async Task SetTextAsync(string? text)
    {
        await GetTopLevel().Clipboard.SetTextAsync(text);
    }

    public async Task ClearAsync()
    {
        await GetTopLevel().Clipboard.ClearAsync();
    }

    public async Task SetDataObjectAsync(IDataObject data)
    {
        await GetTopLevel().Clipboard.SetDataObjectAsync(data);
    }

    public async Task<string[]> GetFormatsAsync()
    {
        return await GetTopLevel().Clipboard.GetFormatsAsync();
    }

    public async Task<object?> GetDataAsync(string format)
    {
        return await GetTopLevel().Clipboard.GetDataAsync(format);
    }
}
