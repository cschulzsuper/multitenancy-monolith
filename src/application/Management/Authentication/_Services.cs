using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddAuthenticationManagement(this IServiceCollection services)
    {
        services.AddScoped<IIdentityManager, IdentityManager>();
        services.AddScoped<IIdentityVerificationManager, IdentityVerificationManager>();

        return services;
    }
}