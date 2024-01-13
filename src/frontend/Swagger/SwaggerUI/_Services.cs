using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Swagger.SwaggerUI;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddSwaggerJsonWebServiceClients(this IServiceCollection services)
    {
        services.AddScoped<SwaggerJsonClientFactory>();
        services.AddScoped<SwaggerJsonClientTokenProvider>();

        return services;
    }
}

