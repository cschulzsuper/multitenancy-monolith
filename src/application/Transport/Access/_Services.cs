using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddAccessTransport(this IServiceCollection services)
    {
        services.AddScoped<IAccountGroupRequestHandler, AccountGroupRequestHandler>();
        services.AddScoped<IAccountMemberRequestHandler, AccountMemberRequestHandler>();
        services.AddScoped<IAccountRegistrationCommandHandler, AccountRegistrationCommandHandler>();
        services.AddScoped<IAccountRegistrationRequestHandler, AccountRegistrationRequestHandler>();
        services.AddScoped<IContextAccountMemberCommandHandler, ContextAccountMemberCommandHandler>();
        services.AddScoped<IContextAccountRegistrationCommandHandler, ContextAccountRegistrationCommandHandler>();

        return services;
    }
}