using System;

namespace LyricStudio.Core.MusicTag;

[Serializable]
public class MusicInfo
{
    public string Name { get; set; }

    public string Artist { get; set; }

    public string AlbumName { get; set; }

    public byte[] AlbumImage { get; set; }

    public string FilePath { get; set; }

    public int Duration { get; set; }

    public double BitRate { get; set; }

    public MusicInfoStatus Status { get; set; }

    public MusicInfo()
    {
        Status = MusicInfoStatus.WaitingDownload;
    }

    public MusicInfo(string name, string artist) : this()
    {
        Name = name;
        Artist = artist;
    }

    public MusicInfo(string name, string artist, string albumName, byte[] albumImage, string filePath) : this(name, artist)
    {
        AlbumName = albumName;
        AlbumImage = albumImage;
        FilePath = filePath;
    }
}

[Flags]
public enum MusicInfoStatus
{
    WaitingDownload,
    DownloadCompleted,
    MusicTagInvalid
}
