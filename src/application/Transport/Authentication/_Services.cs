using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddAuthenticationTransport(this IServiceCollection services)
    {
        services.AddScoped<IIdentityCommandHandler, IdentityCommandHandler>();
        services.AddScoped<IIdentityRequestHandler, IdentityRequestHandler>();

        return services;
    }
}