using FFME;
using FFME.Common;
using FFmpeg.AutoGen;
using System;
using System.IO;

namespace LyricStudio.Core.Player;

public class FFMEAudioPlayer : IAudioPlayer
{
    public string FileName { get; protected set; }
    private MediaElement MediaElement { get; }

    public event EventHandler<double> PositionChanged;

    static FFMEAudioPlayer()
    {
        // Change the default location of the ffmpeg binaries (same directory as application)
        // You can get the 64-bit binaries here: https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-full-shared.7z
        Library.FFmpegDirectory = Path.GetFullPath(".");

        // You can pick which FFmpeg binaries are loaded. See issue #28
        // For more specific control (issue #414) you can set Library.FFmpegLoadModeFlags to:
        // FFmpegLoadMode.LibraryFlags["avcodec"] | FFmpegLoadMode.LibraryFlags["avfilter"] | ... etc.
        // Full Features is already the default.
        Library.FFmpegLoadModeFlags = FFmpegLoadMode.FullFeatures;

        // Multi-threaded video enables the creation of independent
        // dispatcher threads to render video frames. This is an experimental feature
        // and might become deprecated in the future as no real performance enhancements
        // have been detected.
        Library.EnableWpfMultiThreadedVideo =
            false; // !System.Diagnostics.Debugger.IsAttached; // test with true and false
    }

    public FFMEAudioPlayer()
    {
        MediaElement = new();
    }

    public void Dispose()
    {
        if (MediaElement != null)
        {
            MediaElement.PositionChanged -= OnPositionChanged;
            MediaElement?.Dispose();
        }
    }

    public void Open(string fileName)
    {
        FileName = fileName;
        MediaElement.Open(new Uri(fileName, UriKind.Absolute));
        MediaElement.PositionChanged += OnPositionChanged;
    }

    public void Play() => MediaElement.Play();

    public void Pause() => MediaElement.Pause();

    public void Stop() => MediaElement.Stop();

    public void SeekTo(double second) => MediaElement.Seek(TimeSpan.FromSeconds(second));

    public double Length => MediaElement.NaturalDuration!.Value.TotalSeconds;

    public double Position => MediaElement.ActualPosition!.Value.TotalSeconds;

    public int Volume
    {
        get => (int)(MediaElement.Volume * 100d);
        set => MediaElement.Volume = value / 100d;
    }

    public double Rate
    {
        get => MediaElement.SpeedRatio;
        set => MediaElement.SpeedRatio = value;
    }

    public long AudioDelay => throw new NotImplementedException();

    public AudioPlayerState State => MediaElement.MediaState switch
    {
        MediaPlaybackState.Manual => AudioPlayerState.None,
        MediaPlaybackState.Play => AudioPlayerState.Playing,
        MediaPlaybackState.Close => AudioPlayerState.Stopped,
        MediaPlaybackState.Pause => AudioPlayerState.Paused,
        MediaPlaybackState.Stop => AudioPlayerState.Stopped,
        _ => AudioPlayerState.None,
    };

    private void OnPositionChanged(object? sender, PositionChangedEventArgs e)
    {
        PositionChanged?.Invoke(this, e.Position.TotalSeconds / Length);
    }
}
