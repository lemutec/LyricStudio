﻿using FFME.Common;

namespace FFME.Avalonia.Common
{
    /// <summary>
    /// Provides the audio samples rendering payload as event arguments.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public sealed class RenderingAudioEventArgs : RenderingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderingAudioEventArgs" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="length">The length.</param>
        /// <param name="latency">The latency between the buffer position and the real-time playback clock.</param>
        /// <param name="engineState">The engine.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="pts">The unadjusted PTS of the frame in stream Time Base units.</param>
        internal RenderingAudioEventArgs(
            byte[] buffer, int length, TimeSpan latency, IMediaEngineState engineState, StreamInfo stream, TimeSpan startTime, TimeSpan duration, TimeSpan clock, long pts)
            : base(engineState, stream, startTime, duration, clock, pts)
        {
            Buffer = buffer;
            BufferLength = length;
            SampleRate = Constants.AudioSampleRate;
            ChannelCount = Constants.AudioChannelCount;
            BitsPerSample = Constants.AudioBitsPerSample;
            Latency = latency;
        }

        /// <summary>
        /// Gets the latency between the audio buffer position and the real-time playback clock.
        /// </summary>
        public TimeSpan Latency { get; }

        /// <summary>
        /// Gets a the raw data buffer going into the audio device.
        /// Samples are provided in PCM 16-bit signed, interleaved stereo.
        /// </summary>
        public IReadOnlyList<byte> Buffer { get; }

        /// <summary>
        /// Gets the length in bytes of the samples buffer.
        /// </summary>
        public int BufferLength { get; }

        /// <summary>
        /// Gets the number of samples in 1 second.
        /// </summary>
        public int SampleRate { get; }

        /// <summary>
        /// Gets the number of channels.
        /// </summary>
        public int ChannelCount { get; }

        /// <summary>
        /// Gets the number of bits per sample.
        /// </summary>
        public int BitsPerSample { get; }

        /// <summary>
        /// Gets the number of samples in the buffer for all channels.
        /// </summary>
        public int Samples => BufferLength / (BitsPerSample / 8);

        /// <summary>
        /// Gets the number of samples in the buffer per channel.
        /// </summary>
        public int SamplesPerChannel => Samples / ChannelCount;

        /// <summary>
        /// Gets a the raw data buffer going into the audio device.
        /// Samples are provided in PCM 16-bit signed, interleaved stereo.
        /// </summary>
        /// <returns>The buffer data as an array.</returns>
        public byte[] GetBufferData() => Buffer as byte[];
    }
}
