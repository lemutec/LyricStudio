using Avalonia.Controls;
using Avalonia.Metadata;
using System.Globalization;

namespace Fischless.Design.Converters;

/// <summary>
/// StringToObjectConverter can be used to select different resources based on given string name.
/// This can be particularly useful if a string key needs to represent an image on the user interface.
///
/// Use the Items property to create a ResourceDictionary which contains object-to-string-name mappings.
///
/// Check out following example:
/// <example>
///    <ResourceDictionary>
///      <BitmapImage x:Key="Off" UriSource="/Resources/Images/stop.png" />
///      <BitmapImage x:Key="On" UriSource="/Resources/Images/play.png" />
///      <BitmapImage x:Key="Maybe" UriSource="/Resources/Images/pause.png" />
///    </ResourceDictionary>
/// </example>
/// Source: http://stackoverflow.com/questions/2787725/how-to-display-different-enum-icons-using-xaml-only
/// </summary>
public class StringToObjectConverter : SingletonConverterBase<StringToObjectConverter>
{
    [Content]
    public ResourceDictionary Items { get; set; }

    protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string key)
        {
            if (this.Items != null && ContainsKey(this.Items, key))
            {
                return this.Items[key];
            }
        }

        return UnsetValue;
    }

    private static bool ContainsKey(ResourceDictionary dict, string key)
    {
        return dict.ContainsKey(key);
    }
}
