﻿using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Fischless.Linq;
using Fischless.Mapper;
using Fischless.Mvvm;
using Fischless.Win32;
using Fischless.Win32.SystemDialog;
using LibVLCSharp.Shared;
using LyricStudio.Core.AudioTrack;
using LyricStudio.Core.Configuration;
using LyricStudio.Core.LyricTrack;
using LyricStudio.Core.MusicTag;
using LyricStudio.Core.Player;
using LyricStudio.Models;
using LyricStudio.Models.Audios;
using LyricStudio.Models.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace LyricStudio.ViewModels;

public partial class HomePageViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    private LyricEditMode mode = LyricEditMode.ListView;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMediaAvailable))]
    private IAudioPlayer audioPlayer = null!;

    private readonly DispatcherTimer timer = new();

    private readonly LrcManager lrcManager = new();

    [ObservableProperty]
    private ObservableCollectionEx<ObservableLrcLine> lrcLines = [];

    /// <summary>
    /// Used for <see cref="LyricEditMode.ListView"/>
    /// </summary>
    [ObservableProperty]
    private ObservableLrcLine selectedlrcLine = null!;

    partial void OnSelectedlrcLineChanged(ObservableLrcLine value)
    {
        EditinglrcLine = value?.ToString();
    }

    /// <summary>
    /// Used for <see cref="LyricEditMode.ListView"/>
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LrcLines))]
    private string editinglrcLine = null!;

    partial void OnEditinglrcLineChanged(string value)
    {
        if (SelectedlrcLine != null)
        {
            LrcLine v = LrcLine.Parse(value);

            SelectedlrcLine.LrcText = v.LrcText;
            SelectedlrcLine.LrcTime = v.LrcTime;
        }
    }

    /// <summary>
    /// Used for <see cref="LyricEditMode.ListView"/>
    /// </summary>
    [ObservableProperty]
    private string currentLrcText = string.Empty;

    /// <summary>
    /// Used for <see cref="LyricEditMode.TextBox"/>
    /// </summary>
    [ObservableProperty]
    private string lyricText = null!;

    [ObservableProperty]
    private bool isPlaying = false;

    [ObservableProperty]
    private double currentTime = default;

    [ObservableProperty]
    private double totalTime = default;

    [ObservableProperty]
    private double position = default;

    [ObservableProperty]
    private double volume = ConfigurationKeys.Volume.Get();

    partial void OnVolumeChanged(double value)
    {
        ConfigurationKeys.Volume.Set(value);
        Config.Configer?.Save(AppConfig.SettingsFile);
        if (IsMediaAvailable)
        {
            AudioPlayer.Volume = (int)(value * 100d);
        }
    }

    [ObservableProperty]
    private double rate = 1d;

    partial void OnRateChanged(double value)
    {
        if (IsMediaAvailable)
        {
            AudioPlayer.Rate = value;
        }
    }

    [ObservableProperty]
    private string tagName = string.Empty;

    [ObservableProperty]
    private string tagArtist = string.Empty;

    [ObservableProperty]
    private string tagAlbumName = string.Empty;

    [ObservableProperty]
    private byte[] tagAlbumImage = null!;

    [ObservableProperty]
    private string lyricFilePath = null!;

    [ObservableProperty]
    private string musicFilePath = null!;

    [ObservableProperty]
    private int tagDuration = 0;

    [ObservableProperty]
    private string tagBitRate = string.Empty;

    [ObservableProperty]
    private List<AudioVolume> volumes = [];

    [ObservableProperty]
    private bool isSaved = true;

    [ObservableProperty]
    private bool isReading = false;

    protected bool IsMediaAvailable => AudioPlayer != null;

    [ObservableProperty]
    private bool isUseTwoDigitTimeCode = ConfigurationKeys.IsUseTwoDigitTimeCode.Get();

    [ObservableProperty]
    private int flagTimeOffset = ConfigurationKeys.FlagTimeOffset.Get();

    [ObservableProperty]
    private double shortShiftSeconds = ConfigurationKeys.ShortShiftSeconds.Get();

    [ObservableProperty]
    private double longShiftSeconds = ConfigurationKeys.LongShiftSeconds.Get();

    public HomePageViewModel()
    {
        timer.Tick += (_, _) => OnTick();
        timer.Interval = TimeSpan.FromMicroseconds(20d);
        timer.Start();

        WeakReferenceMessenger.Default.Register<FileDropMessage>(this, async (sender, msg) =>
        {
            await OpenFilesAsync(msg.FileNames);
        });
    }

    [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
    public void Dispose()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        timer?.Stop();
    }

    private void OnTick()
    {
        if (!IsMediaAvailable)
        {
            return;
        }
        SyncHightlight();
    }

    private void SyncHightlight()
    {
        LrcLine line = LrcHelper.GetNearestLrc(LrcLines, TimeSpan.FromSeconds(CurrentTime));

        CurrentLrcText = line?.LrcText ?? string.Empty;

        (LrcLines as IEnumerable<ObservableLrcLine>).ForEach(v => v.IsHightlight = false);
        if (line is ObservableLrcLine oLine)
        {
            oLine.IsHightlight = true;
        }
    }

    [SuppressMessage("Style", "IDE0074:Use compound assignment")]
    private async Task OpenFilesAsync(params string[] fileNames)
    {
        IsReading = true;
        Stop();

        try
        {
            string? lyricFile = fileNames.Where(f => !string.IsNullOrWhiteSpace(f) && (new FileInfo(f).Extension?.Equals(".lrc", StringComparison.OrdinalIgnoreCase) ?? false)).FirstOrDefault();
            string? musicFile = fileNames.Where(f => MediaInfoAudio.HasAudioTrack(f)).FirstOrDefault();

            if (lyricFile == null)
            {
                lyricFile = fileNames.Where(f => !string.IsNullOrWhiteSpace(f) && (new FileInfo(f).Extension?.Equals(".ass", StringComparison.OrdinalIgnoreCase) ?? false)).FirstOrDefault();
            }

            if (lyricFile == null && musicFile == null)
            {
                return;
            }

            if (lyricFile == null)
            {
                string? lyricFileHatena = Path.ChangeExtension(musicFile, "lrc");

                if (File.Exists(lyricFileHatena))
                {
                    lyricFile = lyricFileHatena;
                }
                else
                {
                    lyricFileHatena = Path.ChangeExtension(musicFile, "ass");

                    if (File.Exists(lyricFileHatena))
                    {
                        lyricFile = lyricFileHatena;
                    }
                }
            }

            if (lyricFile != null)
            {
                IsSaved = true;

                if (Path.GetExtension(lyricFile).Equals(".ass", StringComparison.OrdinalIgnoreCase))
                {
                    if (Ass2lrc.AssToLyric(LrcHelper.ReadAllText(lyricFile), out string lyricText))
                    {
                        lyricFile = Path.ChangeExtension(lyricFile, "lrc");

                        LyricText = lyricText;
                        IsSaved = false;
                    }
                    else
                    {
                        lyricFile = null;
                    }
                }
                else
                {
                    LyricText = LrcHelper.ReadAllText(lyricFile);
                }

                LyricFilePath = lyricFile;
                lrcManager.LoadText(LyricText);
                LrcLines.Reset(lrcManager.LrcList.Select(v => MapperProvider.Map<LrcLine, ObservableLrcLine>(v)));
            }

            if (musicFile == null)
            {
                string? musicFileHatena = Directory.GetFiles(Path.GetDirectoryName(lyricFile), $"{Path.GetFileNameWithoutExtension(lyricFile)}.*")
                    .Where(f => !f.EndsWith(".lrc", StringComparison.OrdinalIgnoreCase) && !f.EndsWith(".aas", StringComparison.OrdinalIgnoreCase))
                    .Where(MediaInfoAudio.HasAudioTrack)
                    .FirstOrDefault();

                if (musicFileHatena != null)
                {
                    musicFile = musicFileHatena;
                }
            }

            if (musicFile != null)
            {
                MusicFilePath = musicFile;

                TagAlbumImage = null!;
                MusicInfo musicInfo = await Task.Run(() =>
                {
                    MusicInfoLoaderByTagLib musicInfoLoader = new();
                    MusicInfo musicInfo = musicInfoLoader.Load(musicFile);

                    if (musicInfo.Status != MusicInfoStatus.MusicTagInvalid)
                    {
                        musicInfo.BitRate = MediaInfoAudio.GetAudioBitRate(musicFile) / 1000d;
                        musicInfo.Volumes = AudioInfoProvider.GetAudioVolume(musicFile).ToList();
                        musicInfo.TotalTime = AudioInfoProvider.GetTotalTime(musicFile);
                    }
                    return musicInfo;
                });

                if (musicInfo.Status != MusicInfoStatus.MusicTagInvalid)
                {
                    TagAlbumName = musicInfo.AlbumName;
                    TagName = musicInfo.Name;
                    TagArtist = musicInfo.Artist;
                    TagAlbumImage = musicInfo.AlbumImage;
                    TagDuration = musicInfo.Duration;
                    TagBitRate = $"{musicInfo.BitRate} kbps";
                    Volumes = musicInfo.Volumes;
                    TotalTime = musicInfo.TotalTime;
                }

                if (IsMediaAvailable)
                {
                    AudioPlayer.PositionChanged -= OnPositionChanged;
                    AudioPlayer.Dispose();
                    AudioPlayer = null!;
                }
                AudioPlayer = new AudioPlayer(musicFile);
                AudioPlayer.PositionChanged += OnPositionChanged;
            }

            SyncWindowTitle();
        }
        catch (Exception e)
        {
            _ = e;
        }
        finally
        {
            IsReading = false;
        }
    }

    private void SyncWindowTitle()
    {
        if (string.IsNullOrWhiteSpace(MusicFilePath) && string.IsNullOrWhiteSpace(LyricFilePath))
        {
            return;
        }

        string fileName = new FileInfo(Path.ChangeExtension(MusicFilePath, "lrc")).Name;

        if (string.IsNullOrWhiteSpace(LyricFilePath) || !IsSaved)
        {
            fileName = $"{fileName}•";
        }

        WeakReferenceMessenger.Default.Send(new GlobalMessage(this, GlobalCommand.ChangeMainWindowTitle, fileName));
    }

    [RelayCommand]
    public void Play()
    {
        if (!IsMediaAvailable)
        {
            return;
        }

        switch (AudioPlayer.State)
        {
            case AudioPlayerState.Buffering:
            case AudioPlayerState.Opening:
            case AudioPlayerState.Error:
                break;

            case AudioPlayerState.Playing:
            case AudioPlayerState.Ended:
                IsPlaying = false;
                AudioPlayer.Pause();
                break;

            case AudioPlayerState.None:
            case AudioPlayerState.Paused:
            case AudioPlayerState.Stopped:
                IsPlaying = true;
                AudioPlayer.Volume = (int)(Volume * 100d);
                AudioPlayer.Rate = Rate;
                IsPlaying = true;
                AudioPlayer.Play();
                break;
        }
    }

    [RelayCommand]
    public void Stop()
    {
        if (!IsMediaAvailable)
        {
            return;
        }

        CurrentTime = default;
        Position = default;
        IsPlaying = false;
        AudioPlayer.Stop();
    }

    [RelayCommand]
    public void ShortShiftLeft()
    {
        SeekInSeconds(CurrentTime - ConfigurationKeys.ShortShiftSeconds.Get());
    }

    [RelayCommand]
    public void ShortShiftRight()
    {
        SeekInSeconds(CurrentTime + ConfigurationKeys.ShortShiftSeconds.Get());
    }

    [RelayCommand]
    public void LongShiftLeft()
    {
        SeekInSeconds(CurrentTime - ConfigurationKeys.LongShiftSeconds.Get());
    }

    [RelayCommand]
    public void LongShiftRight()
    {
        SeekInSeconds(CurrentTime + ConfigurationKeys.LongShiftSeconds.Get());
    }

    public void SeekInSeconds(double second)
    {
        if (!IsMediaAvailable)
        {
            return;
        }

        AudioPlayer.SeekTo(second);
    }

    public void SeekInPosition(double position)
    {
        SeekInSeconds(position * TotalTime);
    }

    private void OnPositionChanged(object? sender, double position)
    {
        CurrentTime = position * TotalTime;
        Position = position;
    }

    [RelayCommand]
    public void Flag()
    {
        if (!IsMediaAvailable)
        {
            return;
        }
    }

    [RelayCommand]
    public void Exchange()
    {
        LyricEditMode targetMode = Mode == LyricEditMode.ListView ? LyricEditMode.TextBox : LyricEditMode.ListView;

        if (targetMode == LyricEditMode.ListView)
        {
            // TODO
        }
        else if (targetMode == LyricEditMode.TextBox)
        {
            // TODO
        }

        Mode = targetMode;
    }

    [RelayCommand]
    [SupportedOSPlatform("Windows")]
    [SupportedOSPlatform("MacOS")]
    public async Task OpenAudioFile()
    {
        if (await new OpenFileDialog() { Title = "打开音乐文件" }.ShowAsync() is OpenFileDialogResult result)
        {
            string name = result.Item.FileInfo.FullName;
        }
    }

    [RelayCommand]
    [SupportedOSPlatform("Windows")]
    [SupportedOSPlatform("MacOS")]
    public async Task OpenLyricFile()
    {
        if (await new OpenFileDialog()
        {
            Title = "打开歌词文件",
            Filters = [
                new FileDialogFilter()
                {
                    Name = "歌词文件",
                    Extensions = [.. LrcHelper.LyricExtensions]
                },
            ]
        }.ShowAsync() is OpenFileDialogResult result)
        {
            string name = result.Item.FileInfo.FullName;
        }
    }

    [RelayCommand]
    public void SaveLyricFile()
    {
    }

    [RelayCommand]
    public void CopyLyric()
    {
    }

    [RelayCommand]
    public void CloseFiles()
    {
    }

    [RelayCommand]
    public void Undo()
    {
    }

    [RelayCommand]
    public void Redo()
    {
    }

    [RelayCommand]
    public void AddLine()
    {
    }

    [RelayCommand]
    public void RemoveLine()
    {
    }

    [RelayCommand]
    public void MoveUpLine()
    {
    }

    [RelayCommand]
    public void MoveDownLine()
    {
    }

    [RelayCommand]
    public void ShowInfo()
    {
    }

    /// <summary>
    /// Caused by double tapped
    /// </summary>
    [RelayCommand]
    public void PlaySeekLyric()
    {
        if (SelectedlrcLine is null)
        {
            return;
        }

        if (SelectedlrcLine.LrcTime.HasValue)
        {
            SeekInSeconds(SelectedlrcLine.LrcTime.Value.TotalSeconds);
        }
    }

    /// <summary>
    /// Change to [00:00.00]
    /// </summary>
    [RelayCommand]
    public void ResetTimeMark()
    {
    }

    /// <summary>
    /// Remove like [00:00.00]
    /// </summary>
    [RelayCommand]
    public void ClearTimeMark()
    {
    }

    [RelayCommand]
    public void ShiftTimecode()
    {
    }

    [RelayCommand]
    public void Tranlate()
    {
    }

    [RelayCommand]
    public void TranlateSimplifiedToTraditional()
    {
    }

    [RelayCommand]
    public void TranlateTraditionalToSimplified()
    {
    }

    [RelayCommand]
    public void TranlateKanjiToRomaji()
    {
    }

    [RelayCommand]
    public void FixKanji()
    {
    }

    [RelayCommand]
    public void Settings()
    {
    }
}

[ObservableObject]
public partial class ObservableLrcLine : LrcLine
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LrcTimeText))]
    [NotifyPropertyChangedFor(nameof(PreviewText))]
    private TimeSpan? lrcTime = default;

    partial void OnLrcTimeChanged(TimeSpan? value)
    {
        base.LrcTime = value;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LrcTimeText))]
    [NotifyPropertyChangedFor(nameof(PreviewText))]
    private string lrcText = default;

    partial void OnLrcTextChanged(string value)
    {
        base.LrcText = value;
    }

    [ObservableProperty]
    [property: NotMapped]
    private bool isHightlight = false;
}

public enum LyricEditMode
{
    ListView,
    TextBox,
}
