using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddAuthorizationTransport(this IServiceCollection services)
    {
        services.AddScoped<IMemberCommandHandler, MemberCommandHandler>();
        services.AddScoped<IMemberRequestHandler, MemberRequestHandler>();

        return services;
    }
}