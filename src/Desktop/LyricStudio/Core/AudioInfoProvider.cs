﻿using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace LyricStudio.Core;

internal static class AudioInfoProvider
{
    public static IEnumerable<AudioVolume> GetAudioVolume(string fileName)
    {
        using var audioFile = new AudioFileReader(fileName);
        const int blockSize = 1024;
        float[] buffer = new float[blockSize];

        // Calculate the duration of each block
        float blockDuration = (float)blockSize / audioFile.WaveFormat.SampleRate;

        // Calculate the total number of samples in the audio
        long totalSamples = audioFile.Length / (audioFile.WaveFormat.BitsPerSample / 8);

        // Assume the dBFS (decibels relative to full scale) value corresponding to maximum possible volume is 0 dBFS
        const float maxDBFS = 0.0f;

        // Assume the dBFS value corresponding to minimum possible volume is -60 dBFS (a common level for quiet audio)
        const float minDBFS = -60.0f;

        // Calculate the range of dBFS values
        const float dbRange = maxDBFS - minDBFS;

        // Loop through the audio data and calculate volume
        while (audioFile.Position < audioFile.Length)
        {
            // Calculate the time offset of the current block
            float currentTimeOffset = (float)audioFile.Position / audioFile.WaveFormat.SampleRate;

            // Read a block of data from the audio file
            int bytesRead = audioFile.Read(buffer, 0, blockSize);

            if (bytesRead == 0)
            {
                // End of audio file
                break;
            }

            // Calculate the root mean square (RMS) of the audio block
            float sum = 0;
            for (int i = 0; i < bytesRead / (audioFile.WaveFormat.BitsPerSample / 8); i++)
            {
                // Sum of squares
                sum += buffer[i] * buffer[i];
            }
            float rms = (float)Math.Sqrt(sum / (bytesRead / (audioFile.WaveFormat.BitsPerSample / 8))); // RMS

            // Convert volume to decibels (dB)
            // Avoid divide by zero error
            float volumeInDB = 20.0f * (float)Math.Log10(rms + float.Epsilon);

            // Map the dB value to the range of 0 to 100
            float mappedVolume = ((volumeInDB - minDBFS) / dbRange) * 100;

            // Ensure volume is within the range of 0 to 100
            mappedVolume = Math.Max(0, Math.Min(100, mappedVolume));

            yield return new AudioVolume
            {
                DB = volumeInDB,
                Time = currentTimeOffset,
                Volume = mappedVolume,
            };
        }
    }
}

public class AudioVolume
{
    public float Volume { get; init; }
    public float DB { get; init; }
    public float Time { get; init; }

    public override string ToString() => $"[{Time}] {Volume} (in {DB}DB)";
}
