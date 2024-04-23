using System.Threading.Tasks;

namespace LyricStudio.Core.MusicTag;

public interface IMusicInfoLoader
{
    public MusicInfo Load(string musicFilePath);

    public Task<MusicInfo> LoadAsync(string musicFilePath);

    public void Save(MusicInfo musicInfo);

    public Task SaveAsync(MusicInfo musicInfo);
}
