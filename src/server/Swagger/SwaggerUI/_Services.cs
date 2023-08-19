using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger.SwaggerUI;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddWebServiceSwaggerJsonClients(this IServiceCollection services)
    {
        services.AddScoped<SwaggerJsonClientFactory>();
        services.AddScoped<SwaggerJsonClientTokenProvider>();

        return services;
    }
}

