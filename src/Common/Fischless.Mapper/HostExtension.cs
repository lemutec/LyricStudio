using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Fischless.Mapper;

public static class HostExtension
{
    public static IHostBuilder UseMapper(this IHostBuilder app, object hostOrAssembly = null!)
    {
        MapperAssemblyResolver.Register(hostOrAssembly is Assembly assembly ? assembly : hostOrAssembly?.GetType()?.Assembly);
        return app;
    }
}
