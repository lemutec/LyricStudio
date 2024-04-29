using LyricStudio.Models.Audios;
using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace LyricStudio.Core.AudioTrack;

internal static class AudioInfoProvider
{
    /// <summary>
    /// In Seconds
    /// </summary>
    public static double GetTotalTime(string fileName)
    {
        using AudioFileReader audioFile = new(fileName);
        return audioFile.TotalTime.TotalSeconds;
    }

    public static IEnumerable<AudioVolume> GetAudioVolume(string fileName)
    {
        using AudioFileReader audioFile = new(fileName);
        const int blockSize = 1024;
        float[] buffer = new float[blockSize];

        // Calculate the duration of each block
        float blockDuration = (float)blockSize / audioFile.WaveFormat.SampleRate;

        // Calculate the total number of samples in the audio
        long totalSamples = audioFile.Length / (audioFile.WaveFormat.BitsPerSample / 8);

        // Assume the dBFS (decibels relative to full scale) value corresponding to maximum possible volume is 0 dBFS
        const float maxDBFS = 0f;

        // Assume the dBFS value corresponding to minimum possible volume is -30 dBFS (a common level for quiet audio)
        const float minDBFS = -30f;

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
            float sum = 0f;
            for (int i = 0; i < bytesRead / (audioFile.WaveFormat.BitsPerSample / 8); i++)
            {
                // Sum of squares
                sum += buffer[i] * buffer[i];
            }
            float rms = (float)Math.Sqrt(sum / (bytesRead / (audioFile.WaveFormat.BitsPerSample / 8))); // RMS

            // Convert volume to decibels (dB)
            // Avoid divide by zero error
            float volumeInDB = 20f * (float)Math.Log10(rms + float.Epsilon);

            // Map the dB value to the range of 0 to 100
            float mappedVolume = (volumeInDB - minDBFS) / dbRange * 100;

            // Ensure volume is within the range of 0 to 100
            mappedVolume = Math.Max(0f, Math.Min(100f, mappedVolume));

            yield return new AudioVolume()
            {
                DB = volumeInDB,
                Time = currentTimeOffset,
                Volume = mappedVolume,
            };
        }
    }
}
