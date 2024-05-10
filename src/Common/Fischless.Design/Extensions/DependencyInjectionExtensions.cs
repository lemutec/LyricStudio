using Microsoft.Extensions.DependencyInjection;

namespace Fischless.Design.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddView<TWindow, TViewModel>(this IServiceCollection services)
        where TWindow : class
        where TViewModel : class
    {
        return services
            .AddSingleton<TWindow>()
            .AddSingleton<TViewModel>();
    }

    public static IServiceCollection AddDialog<TWindow, TViewModel>(this IServiceCollection services)
        where TWindow : class
        where TViewModel : class
    {
        return services
            .AddTransient<TWindow>()
            .AddTransient<TViewModel>();
    }
}
