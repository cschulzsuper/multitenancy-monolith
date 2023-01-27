using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

public static class _Services
{
    public static IServiceCollection AddAuthorizationTransport(this IServiceCollection services)
    {
        services.AddScoped<IMemberCommandHandler, MemberCommandHandler>();
        services.AddScoped<IMemberRequestHandler, MemberRequestHandler>();

        return services;
    }
}