﻿namespace FFME.Platform;

using Common;
using Diagnostics;
using Engine;
using FFmpeg.AutoGen;
using System;

/// <summary>
/// Connects handlers between the Media Engine event signals and a platform-specific implementation.
/// </summary>
internal interface IMediaConnector
{
    /// <summary>
    /// Creates a renderer of the specified media type.
    /// </summary>
    /// <param name="mediaType">Type of the media.</param>
    /// <param name="mediaCore">The media engine.</param>
    /// <returns>The renderer.</returns>
    IMediaRenderer CreateRenderer(MediaType mediaType, MediaEngine mediaCore);

    /// <summary>
    /// Called when a message is logged.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="LoggingMessage"/> instance containing the event data.</param>
    void OnMessageLogged(MediaEngine sender, LoggingMessage e);

    /// <summary>
    /// Called when the media input is initializing.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="config">The container configuration options.</param>
    /// <param name="mediaSource">The media URL source.</param>
    void OnMediaInitializing(MediaEngine sender, ContainerConfiguration config, string mediaSource);

    /// <summary>
    /// Called when the media input was opened and provides a way to configure component streams.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="mediaOptions">The media options.</param>
    /// <param name="mediaInfo">The media information.</param>
    void OnMediaOpening(MediaEngine sender, MediaOptions mediaOptions, MediaInfo? mediaInfo);

    /// <summary>
    /// Called when a change in media options is requested, such as a change in selected component streams.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="mediaOptions">The media options.</param>
    /// <param name="mediaInfo">The media information.</param>
    void OnMediaChanging(MediaEngine sender, MediaOptions mediaOptions, MediaInfo? mediaInfo);

    /// <summary>
    /// Called when a change in stream components has been completed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="mediaInfo">The media information.</param>
    void OnMediaChanged(MediaEngine sender, MediaInfo? mediaInfo);

    /// <summary>
    /// Called when media has been fully opened and components were created.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="mediaInfo">The media information.</param>
    void OnMediaOpened(MediaEngine sender, MediaInfo? mediaInfo);

    /// <summary>
    /// Called when media has been closed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    void OnMediaClosed(MediaEngine sender);

    /// <summary>
    /// Called when a media failure occurs.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    void OnMediaFailed(MediaEngine sender, Exception e);

    /// <summary>
    /// Called when media has reached the end of the stream.
    /// </summary>
    /// <param name="sender">The sender.</param>
    void OnMediaEnded(MediaEngine sender);

    /// <summary>
    /// Called when packet buffering has started.
    /// </summary>
    /// <param name="sender">The sender.</param>
    void OnBufferingStarted(MediaEngine sender);

    /// <summary>
    /// Called when packet buffering has ended.
    /// </summary>
    /// <param name="sender">The sender.</param>
    void OnBufferingEnded(MediaEngine sender);

    /// <summary>
    /// Called when a seek operation has started.
    /// </summary>
    /// <param name="sender">The sender.</param>
    void OnSeekingStarted(MediaEngine sender);

    /// <summary>
    /// Called when a seek operation has ended.
    /// </summary>
    /// <param name="sender">The sender.</param>
    void OnSeekingEnded(MediaEngine sender);

    /// <summary>
    /// Called when media position changes.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    void OnPositionChanged(MediaEngine sender, TimeSpan oldValue, TimeSpan newValue);

    /// <summary>
    /// Called when the playback status changes.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    void OnMediaStateChanged(MediaEngine sender, MediaPlaybackState oldValue, MediaPlaybackState newValue);

    /// <summary>
    /// Called when a media packet is read from the input context.
    /// </summary>
    /// <param name="packet">The unmanaged packet pointer.</param>
    /// <param name="context">The unmanaged input format context.</param>
    unsafe void OnPacketRead(AVPacket* packet, AVFormatContext* context);

    /// <summary>
    /// Called when a data (non-media) frame is received.
    /// Data packets are immediately converted to data frames as soon as they are read.
    /// </summary>
    /// <param name="dataFrame">The data frame.</param>
    /// <param name="stream">The stream information.</param>
    void OnDataFrameReceived(DataFrame dataFrame, StreamInfo stream);

    /// <summary>
    /// Called when a video frame is decoded.
    /// </summary>
    /// <param name="videoFrame">The unmanaged video frame pointer.</param>
    /// <param name="context">The unmanaged input format context.</param>
    unsafe void OnVideoFrameDecoded(AVFrame* videoFrame, AVFormatContext* context);

    /// <summary>
    /// Called when a video frame is decoded.
    /// </summary>
    /// <param name="audioFrame">The unmanaged audio frame pointer.</param>
    /// <param name="context">The unmanaged input format context.</param>
    unsafe void OnAudioFrameDecoded(AVFrame* audioFrame, AVFormatContext* context);

    /// <summary>
    /// Called when a subtitle is decoded.
    /// </summary>
    /// <param name="subtitle">The unmanaged subtitle pointer.</param>
    /// <param name="context">The unmanaged input format context.</param>
    unsafe void OnSubtitleDecoded(AVSubtitle* subtitle, AVFormatContext* context);
}
