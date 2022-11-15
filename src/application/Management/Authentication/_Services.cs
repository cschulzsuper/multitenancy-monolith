using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public static class _Services
{
    public static IServiceCollection AddAuthenticationManagement(this IServiceCollection services)
    {
        services.AddScoped<IIdentityManager, IdentityManager>();

        return services;
    }
}
