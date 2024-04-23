using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;

namespace Fischless.Design.Resources;

public sealed class ControlsResources : ResourceDictionary
{
    public ControlsResources()
    {
        MergedDictionaries.Add(new ResourceInclude(new Uri("avares://Fischless.Design"))
        {
            Source = new Uri("/Resources/Resources.axaml", UriKind.Relative),
        });
    }
}
