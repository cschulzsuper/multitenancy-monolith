using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IAuthenticationServerProvider, AuthenticationServerProvider>();
        services.AddSingleton<IAllowedClientsProvider, AllowedClientsProvider>();
        services.AddSingleton<ISeedDataProvider, SeedDataProvider>();
        services.AddSingleton<ISwaggerDocsProvider, SwaggerDocsProvider>();
        services.AddSingleton<IWebServicesProvider, WebServicesProvider>();

        return services;
    }
}