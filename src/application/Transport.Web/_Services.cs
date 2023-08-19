using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddWebServiceTransportClients(this IServiceCollection services)
    {
        services.AddScoped<TransportWebServiceClientFactory>();

        return services;
    }
}