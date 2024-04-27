namespace LyricStudio.Models.Audios;

public class AudioVolume
{
    public float Volume { get; init; }
    public float DB { get; init; }
    public float Time { get; init; }

    public override string ToString() => $"[{Time}] {Volume} (in {DB}DB)";
}
