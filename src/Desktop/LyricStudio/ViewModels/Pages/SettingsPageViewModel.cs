using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fischless.Globalization;
using Fischless.Linq;
using FluentAvalonia.Core;
using LyricStudio.Core.Configuration;
using LyricStudio.Models;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;

namespace LyricStudio.ViewModels;

public partial class SettingsPageViewModel : ObservableObject
{
    [ObservableProperty]
    private int languageIndex = (int)SmartEnum<LanguageIndex>.FromDescriptionAttribute(ConfigurationKeys.Language.Get(), Fischless.Globalization.LanguageIndex.Chinese)!;

    partial void OnLanguageIndexChanged(int value)
    {
        string prev = SmartEnum<LanguageIndex>.ToDescriptionAttribute(SmartEnum<LanguageIndex>.FromValue(LanguageIndex));
        string next = SmartEnum<LanguageIndex>.ToDescriptionAttribute(SmartEnum<LanguageIndex>.FromValue(value));

        _ = prev;
        MuiLanguage.SetLanguage(next);
        ConfigurationKeys.Language.Set(next);
        Config.Configer?.Save(AppConfig.SettingsFile);
    }

    public SettingsPageViewModel()
    {
    }

    [RelayCommand]
    private void CheckUpdate()
    {
        try
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://github.com/lemutec/LyricStudio/releases",
                UseShellExecute = true,
            });
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    [RelayCommand]
    private void CheckLicense()
    {
        try
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://github.com/lemutec/LyricStudio/blob/master/LICENSE",
                UseShellExecute = true,
            });
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    [RelayCommand]
    private void OpenSpecialFolder()
    {
        try
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = $"file://{new FileInfo(AppConfig.SettingsFile).DirectoryName}/",
                UseShellExecute = true,
            });
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    [RelayCommand]
    private void OpenLogsFolder()
    {
        try
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = $"file://{AppConfig.LogFolder}/",
                UseShellExecute = true,
            });
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }
}
