namespace LyricStudio.Models.Messages;

public sealed class GlobalMessage(object sender, object? command, object? param = null, object? attachment = null)
{
    public object? Sender { get; set; } = sender;
    public string Command { get; set; } = command?.ToString();
    public object? Param { get; set; } = param;
    public object? Attachment { get; set; } = attachment;
    public bool IsCanceled { get; set; } = false;
}

public enum GlobalCommand
{
    ChangeMainWindowTitle,
}
