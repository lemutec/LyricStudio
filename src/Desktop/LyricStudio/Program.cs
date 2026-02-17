using Avalonia;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using System;
using System.Runtime.Versioning;

namespace LyricStudio;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    => BuildAvaloniaApp()
        .WithInterFont()
        .With(new FontManagerOptions
        {
            DefaultFamilyName = "avares://Fischless.Design/Assets/Fonts/MiSans-Regular.ttf#MiSans",
            FontFallbacks =
            [
                new FontFallback
                {
                    FontFamily = new FontFamily("avares://Fischless.Design/Assets/Fonts/MiSans-Regular.ttf#MiSans")
                }
            ]
        })
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .With(new Win32PlatformOptions())
            .LogToTrace()
            .UseReactiveUI();
}
