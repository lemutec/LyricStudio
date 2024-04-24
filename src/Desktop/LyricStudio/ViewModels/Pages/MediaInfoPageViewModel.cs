using CommunityToolkit.Mvvm.ComponentModel;
using Fischless.Win32;

namespace LyricStudio.ViewModels;

public partial class MediaInfoPageViewModel : ObservableObject
{
    [ObservableProperty]
    public string fileName = string.Empty;

    [ObservableProperty]
    public string mediaInfo = string.Empty;

    public void Reload(string fileName)
    {
        FileName = fileName;
        MediaInfo = MediaInfoProvider.Inform(fileName);
    }
}
