using Microsoft.Extensions.DependencyInjection;

namespace LyricStudio.Core.Configuration;

/// <summary>
/// Extension methods for setting up logging services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ConfigurationServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfigurationImpl config)
    {
        return services.AddSingleton(config);
    }
}
