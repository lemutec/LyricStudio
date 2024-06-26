﻿using FFME.ClosedCaptions;
using FFmpeg.AutoGen;
using FFME.Common;

namespace FFME.Avalonia.Common;

/// <summary>
/// The video rendering event arguments.
/// </summary>
/// <seealso cref="EventArgs" />
public sealed class RenderingVideoEventArgs : RenderingEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RenderingVideoEventArgs" /> class.
    /// </summary>
    /// <param name="bitmap">The bitmap.</param>
    /// <param name="closedCaptions">The closed captions.</param>
    /// <param name="smtpeTimeCode">The smtpe time code.</param>
    /// <param name="pictureNumber">The picture number.</param>
    /// <param name="pictureType">The origination picture type.</param>
    /// <param name="engineState">The engine.</param>
    /// <param name="stream">The stream.</param>
    /// <param name="startTime">The start time.</param>
    /// <param name="duration">The duration.</param>
    /// <param name="clock">The clock.</param>
    /// <param name="pts">The unadjusted, original presentation timestamp (PTS) of the frame.</param>
    internal RenderingVideoEventArgs(
        BitmapDataBuffer bitmap,
        IReadOnlyList<ClosedCaptionPacket> closedCaptions,
        string smtpeTimeCode,
        long pictureNumber,
        AVPictureType pictureType,
        IMediaEngineState engineState,
        StreamInfo stream,
        TimeSpan startTime,
        TimeSpan duration,
        TimeSpan clock,
        long pts)
        : base(engineState, stream, startTime, duration, clock, pts)
    {
        PictureNumber = pictureNumber;
        Bitmap = bitmap;
        SmtpeTimeCode = smtpeTimeCode;
        ClosedCaptions = closedCaptions;
        PictureType = pictureType;
    }

    /// <summary>
    /// Gets the writable bitmap filled with the video frame pixels.
    /// Feel free to capture or change this buffer.
    /// </summary>
    public BitmapDataBuffer Bitmap { get; }

    /// <summary>
    /// Gets the closed caption decoded packets.
    /// </summary>
    public IReadOnlyList<ClosedCaptionPacket> ClosedCaptions { get; }

    /// <summary>
    /// Gets the display picture number (frame number).
    /// If not set by the decoder, this attempts to obtain it by dividing the start time by the
    /// frame duration.
    /// </summary>
    public long PictureNumber { get; }

    /// <summary>
    /// Gets the SMTPE time code.
    /// </summary>
    public string SmtpeTimeCode { get; }

    /// <summary>
    /// Gets the picture type.
    /// </summary>
    public AVPictureType PictureType { get; }
}
