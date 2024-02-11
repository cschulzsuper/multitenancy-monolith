using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddExtensionManagement(this IServiceCollection services)
    {
        services.AddSingleton<IObjectTypeDefinitionProvider, ObjectTypeDefinitionProvider>();

        services.AddScoped<IObjectTypeManager, ObjectTypeManager>();
        services.AddScoped<IDistinctionTypeManager, DistinctionTypeManager>();

        return services;
    }
}