using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IConfigurationProxyProvider, ConfigurationProxyProvider>();

        return services;
    }
}