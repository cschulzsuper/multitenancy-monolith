using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

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