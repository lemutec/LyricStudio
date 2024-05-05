using LibVLCSharp.Shared;
using System;
using FFME.Common;

namespace LyricStudio.Core.Player;

/// <summary>
/// <see cref="VLCAudioPlayer" />
/// <see cref="FFMEAudioPlayer" />
/// </summary>
public interface IAudioPlayer : IDisposable
{
    public event EventHandler<double> PositionChanged;

    public void Play();

    public void Pause();

    public void Stop();

    public void SeekTo(double second);

    /// <summary>
    /// In Seconds
    /// </summary>
    public double Length { get; }

    /// <summary>
    /// In Percentage
    /// </summary>
    public double Position { get; }

    public int Volume { get; set; }

    public double Rate { get; set; }

    /// <summary>
    /// In Milliseconds
    /// </summary>
    public long AudioDelay { get; }

    public AudioPlayerState State { get; }
}

/// <summary>
/// <see cref="VLCState" />
/// <see cref="MediaPlaybackState"/>
/// </summary>
public enum AudioPlayerState
{
    None,
    Opening,
    Buffering,
    Playing,
    Paused,
    Stopped,
    Ended,
    Error,
}
