using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Fischless.Design.Extensions;
using Fischless.Globalization;
using Fischless.Mapper;
using LyricStudio.Core.Configuration;
using LyricStudio.Models;
using LyricStudio.Services;
using LyricStudio.ViewModels;
using LyricStudio.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Resources;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace LyricStudio;

public partial class App : Application
{
    internal static IServiceProvider Services => _host.Services;

    // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    private static readonly IHost _host = Host.CreateDefaultBuilder()
        .UseMapper(typeof(App).Assembly)
        .ConfigureServices((context, services) =>
        {
            AppConfig.LogFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"LyricStudio\log");
            AppConfig.SettingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"LyricStudio\config.json");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(AppConfig.LogFolder, $"LyricStudio_{DateTime.Now:yyyyMMdd}.log"))
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                .CreateLogger();

            Config.Configer = new ConfigurationImpl()
                .LoadSafe(AppConfig.SettingsFile)
                .Also(c => _ = !Path.Exists(AppConfig.SettingsFile) ? c.SaveSafe(AppConfig.SettingsFile) : default);

            MuiLanguage.SetupLanguage(ConfigurationKeys.Language.Get());

            ResourceManager resourceManager = new("Fischless.Globalization.Properties.Resources", typeof(MuiLanguage).Assembly);
            resourceManager.GetString("AboutProgram", new System.Globalization.CultureInfo("en"));

            services.AddSingleton(services);
            services.AddSingleton<IClipboardService, ClipboardService>();
            services.AddSingleton<IClipboard, ClipboardService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddLogging(c => c.AddSerilog(Log.Logger));
            services.AddConfiguration(Config.Configer);
            services.AddView<MainWindow, MainWindowViewModel>();
            services.AddView<MainView, MainViewViewModel>();
            services.AddView<HomePage, HomePageViewModel>();
            services.AddView<MediaInfoPage, MediaInfoPageViewModel>();
            services.AddView<PluginPage, PluginPageViewModel>();
            services.AddView<SettingsPage, SettingsPageViewModel>();
            services.AddDialog<ShareDialog, ShareDialogViewModel>();
            services.AddDialog<ShiftTimecodeDialog, ShiftTimecodeDialogViewModel>();
            services.AddDialog<RomanizationDialog, RomanizationDialogViewModel>();
        })
        .Build();

    [SupportedOSPlatform("Windows")]
    [SupportedOSPlatform("MacOS")]
    public static Window? MainWindow => (Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

    static App()
    {
        GetLogger<App>().LogInformation("Application starting up...");
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = GetService<MainWindow>();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = GetService<MainView>();
        }

        base.OnFrameworkInitializationCompleted();

        TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;

        await _host.StartAsync();
    }

    private static void OnTaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        try
        {
        }
        catch (Exception ex)
        {
            _ = ex;
        }
        finally
        {
            e.SetObserved();
        }
    }

    private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try
        {
            if (e.ExceptionObject is Exception exception)
            {
                ///
            }
        }
        catch (Exception ex)
        {
            _ = ex;
        }
        finally
        {
            ///
        }
    }

    public static ILogger<T>? GetLogger<T>()
    {
        return _host.Services.GetService<ILogger<T>>()!;
    }

    public static IConfigurationImpl? GetConfiger<T>()
    {
        return _host.Services.GetService<IConfigurationImpl>()!;
    }

    public static T? GetService<T>() where T : class
    {
        return _host.Services.GetService(typeof(T)) as T;
    }

    public static object? GetService(Type type)
    {
        return _host.Services.GetService(type);
    }
}
