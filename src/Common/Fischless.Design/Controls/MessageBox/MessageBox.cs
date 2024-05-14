using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Fischless.Design.Controls;

public static class MessageBox
{
    public static Task<MessageBoxResult> InfoAsync(string messageBoxText)
        => ShowAsync(GetActiveWindow(), messageBoxText, null, MessageBoxButton.OK, MessageBoxImage.Information);

    public static Task<MessageBoxResult> InfoAsync(string messageBoxText, string caption)
     => ShowAsync(GetActiveWindow(), messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Information);

    public static Task<MessageBoxResult> InfoAsync(string messageBoxText, string caption, MessageBoxButton button)
     => ShowAsync(GetActiveWindow(), messageBoxText, caption, button, MessageBoxImage.Information);

    public static Task<MessageBoxResult> InfoAsync(Window? owner, string messageBoxText)
      => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, null, MessageBoxButton.OK, MessageBoxImage.Information);

    public static Task<MessageBoxResult> InfoAsync(Window? owner, string messageBoxText, string caption)
      => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Information);

    public static Task<MessageBoxResult> InfoAsync(Window? owner, string messageBoxText, string caption, MessageBoxButton button)
        => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, caption, button, MessageBoxImage.Information);

    public static Task<MessageBoxResult> WarnAsync(string messageBoxText)
        => ShowAsync(GetActiveWindow(), messageBoxText, null, MessageBoxButton.OK, MessageBoxImage.Warning);

    public static Task<MessageBoxResult> WarnAsync(string messageBoxText, string caption)
     => ShowAsync(GetActiveWindow(), messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Warning);

    public static Task<MessageBoxResult> WarnAsync(string messageBoxText, string caption, MessageBoxButton button)
     => ShowAsync(GetActiveWindow(), messageBoxText, caption, button, MessageBoxImage.Warning);

    public static Task<MessageBoxResult> WarnAsync(Window? owner, string messageBoxText)
      => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, null, MessageBoxButton.OK, MessageBoxImage.Warning);

    public static Task<MessageBoxResult> WarnAsync(Window? owner, string messageBoxText, string caption)
      => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Warning);

    public static Task<MessageBoxResult> WarnAsync(Window? owner, string messageBoxText, string caption, MessageBoxButton button)
        => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, caption, button, MessageBoxImage.Warning);

    public static Task<MessageBoxResult> ErrorAsync(string messageBoxText)
        => ShowAsync(GetActiveWindow(), messageBoxText, null, MessageBoxButton.OK, MessageBoxImage.Error);

    public static Task<MessageBoxResult> ErrorAsync(string messageBoxText, string caption)
     => ShowAsync(GetActiveWindow(), messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Error);

    public static Task<MessageBoxResult> ErrorAsync(string messageBoxText, string caption, MessageBoxButton button)
     => ShowAsync(GetActiveWindow(), messageBoxText, caption, button, MessageBoxImage.Error);

    public static Task<MessageBoxResult> ErrorAsync(Window? owner, string messageBoxText)
      => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, null, MessageBoxButton.OK, MessageBoxImage.Error);

    public static Task<MessageBoxResult> ErrorAsync(Window? owner, string messageBoxText, string caption)
      => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Error);

    public static Task<MessageBoxResult> ErrorAsync(Window? owner, string messageBoxText, string caption, MessageBoxButton button)
        => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, caption, button, MessageBoxImage.Error);

    public static Task<MessageBoxResult> QuestionAsync(string messageBoxText)
        => ShowAsync(GetActiveWindow(), messageBoxText, null, MessageBoxButton.YesNo, MessageBoxImage.Question);

    public static Task<MessageBoxResult> QuestionAsync(string messageBoxText, string caption)
     => ShowAsync(GetActiveWindow(), messageBoxText, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

    public static Task<MessageBoxResult> QuestionAsync(string messageBoxText, string caption, MessageBoxButton button)
     => ShowAsync(GetActiveWindow(), messageBoxText, caption, button, MessageBoxImage.Question);

    public static Task<MessageBoxResult> QuestionAsync(Window? owner, string messageBoxText)
      => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, null, MessageBoxButton.YesNo, MessageBoxImage.Question);

    public static Task<MessageBoxResult> QuestionAsync(Window? owner, string messageBoxText, string caption)
      => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

    public static Task<MessageBoxResult> QuestionAsync(Window? owner, string messageBoxText, string caption, MessageBoxButton button)
        => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, caption, button, MessageBoxImage.Question);

    public static Task<MessageBoxResult> ShowAsync(string messageBoxText)
        => ShowAsync(GetActiveWindow(), messageBoxText, null, MessageBoxButton.OK, MessageBoxImage.Information);

    public static Task<MessageBoxResult> ShowAsync(string messageBoxText, string caption)
        => ShowAsync(GetActiveWindow(), messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Information);

    public static Task<MessageBoxResult> ShowAsync(string messageBoxText, string caption, MessageBoxButton button)
        => ShowAsync(GetActiveWindow(), messageBoxText, caption, button, MessageBoxImage.Information);

    public static Task<MessageBoxResult> ShowAsync(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        => ShowAsync(GetActiveWindow(), messageBoxText, caption, button, icon);

    public static Task<MessageBoxResult> ShowAsync(Window? owner, string messageBoxText)
        => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, null, MessageBoxButton.OK, MessageBoxImage.Information);

    public static Task<MessageBoxResult> ShowAsync(Window? owner, string messageBoxText, string caption)
        => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Information);

    public static Task<MessageBoxResult> ShowAsync(Window? owner, string messageBoxText, string caption, MessageBoxButton button)
        => ShowAsync(owner ?? GetActiveWindow(), messageBoxText, caption, button, MessageBoxImage.Information);

    public static async Task<MessageBoxResult> ShowAsync(Window? owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
    {
        owner ??= GetActiveWindow();

        MessageBoxWindow messageBox = new(
            new MessageBoxStandardParams
            {
                ButtonDefinitions = button,
                ContentTitle = caption,
                ContentMessage = messageBoxText,
                Icon = icon,
                WindowIcon = owner?.Icon,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                MaxWidth = 500,
                MinWidth = 120,
                MaxHeight = 800,
                SizeToContent = SizeToContent.WidthAndHeight,
                ShowInCenter = true,
                Topmost = owner?.Topmost ?? false,
            });

        await messageBox.ShowDialog(owner);
        return MessageBoxResult.None;
    }

    private static Window? GetActiveWindow()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.Windows
                .FirstOrDefault(window => window.IsActive && window.ShowActivated);
        }

        return null;
    }
}
