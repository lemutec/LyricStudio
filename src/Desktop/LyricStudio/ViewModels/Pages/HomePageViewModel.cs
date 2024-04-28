using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Fischless.Win32;
using LyricStudio.Core;
using LyricStudio.Core.MusicTag;
using LyricStudio.Models.Audios;
using LyricStudio.Models.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace LyricStudio.ViewModels;

public partial class HomePageViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    private string tagName = string.Empty;

    [ObservableProperty]
    private string tagAlbumName = string.Empty;

    [ObservableProperty]
    private byte[] tagAlbumImage = null!;

    [ObservableProperty]
    private string lyricFilePath = string.Empty;

    [ObservableProperty]
    private string musicFilePath = string.Empty;

    [ObservableProperty]
    private int tagDuration = 0;

    [ObservableProperty]
    private string tagBitRate = string.Empty;

    [ObservableProperty]
    private List<AudioVolume> volumes = [];

    [ObservableProperty]
    private bool isSaved = false;

    public HomePageViewModel()
    {
        WeakReferenceMessenger.Default.Register<FileDropMessage>(this, (sender, msg) =>
        {
            MusicInfoLoaderByTagLib musicInfoLoader = new();

            foreach (string fileName in msg.FileNames)
            {
                try
                {
                    MusicInfo musicInfo = musicInfoLoader.Load(fileName);

                    if (musicInfo.Status != MusicInfoStatus.MusicTagInvalid)
                    {
                        musicInfo.BitRate = MediaInfoAudio.GetAudioBitRate(fileName) / 1000d;
                        TagAlbumName = musicInfo.AlbumName;
                        TagName = musicInfo.Name;
                        TagAlbumImage = musicInfo.AlbumImage;
                        TagDuration = musicInfo.Duration;
                        TagBitRate = $"{musicInfo.BitRate} kbps";
                        Volumes = AudioInfoProvider.GetAudioVolume(fileName).ToList();
                        MusicFilePath = fileName;
                    }
                    ChangeWindowTitle();
                }
                catch (Exception e)
                {
                    _ = e;
                }
            }
        });
    }

    [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
    public void Dispose()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    private void ChangeWindowTitle()
    {
        if (string.IsNullOrWhiteSpace(MusicFilePath) && string.IsNullOrWhiteSpace(LyricFilePath))
        {
            return;
        }

        string fileName = new FileInfo(Path.ChangeExtension(MusicFilePath, "lrc")).Name;

        if (string.IsNullOrWhiteSpace(LyricFilePath) || !IsSaved)
        {
            fileName = $"*{fileName}";
        }

        WeakReferenceMessenger.Default.Send(new GlobalMessage(this, GlobalCommand.ChangeMainWindowTitle, fileName));
    }
}
