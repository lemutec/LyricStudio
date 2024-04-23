using System.IO;
using System.Threading.Tasks;

namespace LyricStudio.Core.Configuration;

public interface IConfigurationImpl : ISettings
{
    public void Load(string fileName);

    public void Load(Stream stream);

    public Task LoadAsync(string fileName);

    public Task LoadAsync(Stream stream);

    public void Save(string fileName);

    public void Save(Stream stream);

    public Task SaveAsync(string fileName);

    public Task SaveAsync(Stream stream);
}
