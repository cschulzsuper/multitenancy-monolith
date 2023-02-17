using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddAuthorizationManagement(this IServiceCollection services)
    {
        services.AddScoped<IMemberManager, MemberManager>();
        services.AddScoped<IMemberVerificationManager, MemberVerificationManager>();

        return services;
    }
}