using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Fischless.Win32;
using LyricStudio.Core;
using LyricStudio.Core.MusicTag;
using LyricStudio.Models.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LyricStudio.ViewModels;

public partial class HomePageViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    private List<AudioVolume> volumes = [];

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
                        Volumes = AudioInfoProvider.GetAudioVolume(fileName).ToList();
                    }
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
}
