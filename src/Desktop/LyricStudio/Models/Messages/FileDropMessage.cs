namespace LyricStudio.Models.Messages;

internal sealed class FileDropMessage(string[] fileNames)
{
    public string[] FileNames { get; } = fileNames;
}
