﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Fischless.Win32;
using LyricStudio.Core;
using LyricStudio.Core.Configuration;
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
using System.Threading.Tasks;

namespace LyricStudio.ViewModels;

public partial class HomePageViewModel : ObservableObject, IDisposable
{
    [SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
    private IAudioPlayer audioPlayer = null!;

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
        if (audioPlayer != null)
        {
            audioPlayer.Volume = (int)(value * 100d);
        }
    }

    [ObservableProperty]
    private double rate = 1d;

    partial void OnRateChanged(double value)
    {
        if (audioPlayer != null)
        {
            audioPlayer.Rate = value;
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

    public HomePageViewModel()
    {
        WeakReferenceMessenger.Default.Register<FileDropMessage>(this, async (sender, msg) =>
        {
            await OpenFilesAsync(msg.FileNames);
        });
    }

    [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
    public void Dispose()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    private async Task OpenFilesAsync(params string[] fileNames)
    {
        IsReading = true;
        Stop();

        try
        {
            string? lyricFile = fileNames.Where(f => !string.IsNullOrWhiteSpace(f) && (new FileInfo(f).Extension?.Equals(".lrc", StringComparison.OrdinalIgnoreCase) ?? false)).FirstOrDefault();
            string? musicFile = fileNames.Where(f => MediaInfoAudio.HasAudioTrack(f)).FirstOrDefault();

            if (lyricFile == null && musicFile == null)
            {
                return;
            }

            TagAlbumImage = null!;

            if (lyricFile == null)
            {
                string? lyricFileHatena = Path.ChangeExtension(musicFile, "lrc");

                if (File.Exists(lyricFileHatena))
                {
                    lyricFile = lyricFileHatena;
                }
            }

            if (lyricFile != null)
            {
                LyricFilePath = lyricFile;
            }

            if (musicFile == null)
            {
                string? musicFileHatena = Directory.GetFiles(Path.GetDirectoryName(lyricFile), $"{Path.GetFileNameWithoutExtension(lyricFile)}.*")
                    .Where(f => !f.EndsWith(".lrc", StringComparison.OrdinalIgnoreCase))
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

                if (audioPlayer != null)
                {
                    audioPlayer.PositionChanged -= OnPositionChanged;
                    audioPlayer.Dispose();
                    audioPlayer = null!;
                }
                audioPlayer = new AudioPlayer(musicFile);
                audioPlayer.PositionChanged += OnPositionChanged;
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
        if (audioPlayer == null)
        {
            return;
        }

        switch (audioPlayer.State)
        {
            case AudioPlayerState.Buffering:
            case AudioPlayerState.Opening:
            case AudioPlayerState.Error:
                break;

            case AudioPlayerState.Playing:
            case AudioPlayerState.Ended:
                IsPlaying = false;
                audioPlayer.Pause();
                break;

            case AudioPlayerState.None:
            case AudioPlayerState.Paused:
            case AudioPlayerState.Stopped:
                IsPlaying = true;
                audioPlayer.Volume = (int)(Volume * 100d);
                audioPlayer.Rate = Rate;
                IsPlaying = true;
                audioPlayer.Play();
                break;
        }
    }

    [RelayCommand]
    public void Stop()
    {
        if (audioPlayer == null)
        {
            return;
        }

        CurrentTime = default;
        Position = default;
        IsPlaying = false;
        audioPlayer.Stop();
    }

    [RelayCommand]
    public void ShortShiftLeft()
    {
        SeekInSeconds(CurrentTime - 2d);
    }

    [RelayCommand]
    public void ShortShiftRight()
    {
        SeekInSeconds(CurrentTime + 2d);
    }

    [RelayCommand]
    public void LongShiftLeft()
    {
        SeekInSeconds(CurrentTime - 5d);
    }

    [RelayCommand]
    public void LongShiftRight()
    {
        SeekInSeconds(CurrentTime + 5d);
    }

    public void SeekInSeconds(double second)
    {
        if (audioPlayer == null)
        {
            return;
        }

        audioPlayer.SeekTo(second);
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
}
