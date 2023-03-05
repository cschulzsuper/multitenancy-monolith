using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddAccessTransport(this IServiceCollection services)
    {
        services.AddScoped<IAccountMemberCommandHandler, AccountMemberCommandHandler>();
        services.AddScoped<IAccountMemberRequestHandler, AccountMemberRequestHandler>();

        return services;
    }
}