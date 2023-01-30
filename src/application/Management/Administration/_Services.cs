using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public static class _Services
{
    public static IServiceCollection AddAdministrationManagement(this IServiceCollection services)
    {
        services.AddSingleton<IObjectTypeDefinitionProvider, ObjectTypeDefinitionProvider>();

        services.AddScoped<IObjectTypeManager, ObjectTypeManager>();
        services.AddScoped<IDistinctionTypeManager, DistinctionTypeManager>();

        return services;
    }
}