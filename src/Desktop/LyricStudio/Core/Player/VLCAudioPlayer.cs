using LibVLCSharp.Shared;
using System;

namespace LyricStudio.Core.Player;

/// <summary>
/// <inheritdoc/>
/// </summary>
[Obsolete]
public sealed class VLCAudioPlayer : LibVLC, IAudioPlayer
{
    public string FileName { get; }
    private Media Media { get; }
    private MediaPlayer MediaPlayer { get; }

    public event EventHandler<double> PositionChanged = null!;

    public VLCAudioPlayer(string fileName)
    {
        FileName = fileName;
        Media = new Media(this, FileName);
        MediaPlayer = new MediaPlayer(Media);
        Log += null!;
        MediaPlayer.PositionChanged += OnPositionChanged;
        MediaPlayer.EnableKeyInput = false;
        MediaPlayer.EnableMouseInput = false;
    }

    public new void Dispose()
    {
        base.Dispose();
        if (MediaPlayer != null)
        {
            MediaPlayer.PositionChanged -= OnPositionChanged;
            MediaPlayer?.Dispose();
        }
        Media?.Dispose();
    }

    public void Play() => MediaPlayer.Play();

    public void Pause() => MediaPlayer.Pause();

    public void Stop() => MediaPlayer.Stop();

    public void SeekTo(double second) => MediaPlayer.SeekTo(TimeSpan.FromSeconds(second));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public double Length => MediaPlayer.Length / 1000d;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public double Position => (double)MediaPlayer.Position;

    public int Volume
    {
        get => MediaPlayer.Volume;
        set => MediaPlayer.Volume = value;
    }

    private double rate = default;

    /// <summary>
    /// <exception cref="Exception">Failed to set rate.</exception>
    /// </summary>
    public double Rate
    {
        get => rate;
        set
        {
            rate = value;
            if (MediaPlayer.SetRate((float)rate) == -1)
            {
                throw new Exception("Failed to set rate.");
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public long AudioDelay => MediaPlayer.AudioDelay;

    public AudioPlayerState State => (AudioPlayerState)MediaPlayer.State;

    private void OnPositionChanged(object? sender, MediaPlayerPositionChangedEventArgs e)
    {
        PositionChanged?.Invoke(this, e.Position);
    }
}
