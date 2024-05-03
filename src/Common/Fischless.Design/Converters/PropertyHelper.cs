using Avalonia;

namespace Fischless.Design.Converters;

#pragma warning disable AVP1001

static class PropertyHelper
{
    public static AvaloniaProperty Create<T, TParent>(string name, T defaultValue) where TParent : AvaloniaObject =>
        AvaloniaProperty.Register<TParent, T>(name, defaultValue);

    public static AvaloniaProperty Create<T, TParent>(string name) where TParent : AvaloniaObject
        => Create<T, TParent>(name, default);
}
