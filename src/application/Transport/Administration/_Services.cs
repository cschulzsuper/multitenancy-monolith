using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddAdministrationTransport(this IServiceCollection services)
    {
        services.AddSingleton<IObjectTypeRequestHandler, ObjectTypeRequestHandler>();
        services.AddScoped<IObjectTypeCustomPropertyRequestHandler, ObjectTypeCustomPropertyRequestHandler>();

        services.AddScoped<IDistinctionTypeRequestHandler, DistinctionTypeRequestHandler>();
        services.AddScoped<IDistinctionTypeCustomPropertyRequestHandler, DistinctionTypeCustomPropertyRequestHandler>();

        return services;
    }
}