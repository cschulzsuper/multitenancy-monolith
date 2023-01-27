using ChristianSchulz.MultitenancyMonolith.Application.Business;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public static class _Services
{
    public static IServiceCollection AddAdministrationTransport(this IServiceCollection services)
    {
        services.AddSingleton<IAggregateTypeRequestHandler, AggregateTypeRequestHandler>();
        services.AddScoped<IAggregateTypeCustomPropertyRequestHandler, AggregateTypeCustomPropertyRequestHandler>();

        services.AddScoped<IDistinctionTypeRequestHandler, DistinctionTypeRequestHandler>();
        services.AddScoped<IDistinctionTypeCustomPropertyRequestHandler, DistinctionTypeCustomPropertyRequestHandler>();

        return services;
    }
}