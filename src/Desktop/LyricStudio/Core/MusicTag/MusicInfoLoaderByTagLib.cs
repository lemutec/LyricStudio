using System.IO;
using System.Threading.Tasks;
using TagLib;
using TagFile = TagLib.File;
using TagPicture = TagLib.Picture;

namespace LyricStudio.Core.MusicTag;

public class MusicInfoLoaderByTagLib : IMusicInfoLoader
{
    public MusicInfo Load(string musicFilePath)
    {
        MusicInfo result = new();
        string? fileName = Path.GetFileNameWithoutExtension(musicFilePath);

        TagFile tagFile = null;
        try
        {
            tagFile = TagFile.Create(musicFilePath);
            result.Name = tagFile.Tag.Title;
            result.Artist = tagFile.Tag.FirstPerformer;
            if (!string.IsNullOrEmpty(tagFile.Tag.FirstAlbumArtist)) result.Artist = tagFile.Tag.FirstAlbumArtist;
            result.AlbumName = tagFile.Tag.Album;
            result.Duration = (int)tagFile.Properties.Duration.TotalMilliseconds;
        }
        catch (CorruptFileException)
        {
            result.Status = MusicInfoStatus.MusicTagInvalid;
        }
        finally
        {
            if (string.IsNullOrEmpty(result.Name))
            {
                result.Name = fileName;
            }
            if (string.IsNullOrEmpty(result.Artist))
            {
                result.Artist = fileName;
            }

            if (tagFile?.Tag.Pictures.Length > 0)
            {
                result.AlbumImage = tagFile.Tag.Pictures[0].Data.Data;
            }

            result.FilePath = musicFilePath;
            tagFile?.Dispose();
        }

        return result;
    }

    public Task<MusicInfo> LoadAsync(string musicFilePath)
    {
        return Task.FromResult(Load(musicFilePath));
    }

    public void Save(MusicInfo musicInfo)
    {
        TagFile tagFile = TagFile.Create(musicInfo.FilePath);

        tagFile.Tag.Title = musicInfo.Name;
        tagFile.Tag.Performers = new[] { musicInfo.Artist };
        tagFile.Tag.Album = musicInfo.AlbumName;
        tagFile.Tag.Pictures = new IPicture[] { new TagPicture(musicInfo.AlbumImage) };

        tagFile.Save();
    }

    public Task SaveAsync(MusicInfo musicInfo)
    {
        Save(musicInfo);

        return Task.FromResult(default(int));
    }
}
