using Avalonia;
using Avalonia.Markup.Xaml;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Fischless.Design.Markups;

public sealed class ServiceLocatorExtension : MarkupExtension
{
    public Type Type { get; set; } = null!;

    public ServiceLocatorExtension()
    {
    }

    [SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        _ = Type ?? throw new ArgumentNullException(nameof(Type));

        foreach (MethodInfo? method in Application.Current.GetType().GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (method.Name == "GetService" && !method.IsGenericMethodDefinition)
            {
                return method.Invoke(null, [Type]);
            }
        }
        return null;
    }
}
