using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddExtensionTransport(this IServiceCollection services)
    {
        services.AddSingleton<IObjectTypeRequestHandler, ObjectTypeRequestHandler>();
        services.AddScoped<IObjectTypeCustomPropertyRequestHandler, ObjectTypeCustomPropertyRequestHandler>();

        services.AddScoped<IDistinctionTypeRequestHandler, DistinctionTypeRequestHandler>();
        services.AddScoped<IDistinctionTypeCustomPropertyRequestHandler, DistinctionTypeCustomPropertyRequestHandler>();

        return services;
    }
}