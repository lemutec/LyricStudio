using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using System.Diagnostics.CodeAnalysis;

namespace Fischless.Design.Controls;

public class ViewLocator : INavigationPageFactory
{
    public static IServiceProvider Services { get; set; } = null!;

    [SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
    public Control GetPage(Type srcType)
    {
        _ = Services ?? throw new ArgumentNullException(nameof(Services));
        if (Services.GetService(srcType) is Control vf)
        {
            return vf;
        }
        return default;
    }

    public Control GetPageFromObject(object target)
    {
        if (target is Type viewType)
        {
            return GetPage(viewType);
        }

        if (Attribute.GetCustomAttribute(target.GetType(), typeof(ViewAttribute)) is not ViewAttribute viewAttr)
        {
            throw new InvalidOperationException("View not found for " + target.GetType().FullName);
        }

        return GetPage(viewAttr.ViewType);
    }
}
