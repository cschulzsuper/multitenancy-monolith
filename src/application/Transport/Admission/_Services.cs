using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddAdmissionTransport(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationIdentityRequestHandler, AuthenticationIdentityRequestHandler>();
        services.AddScoped<IAuthenticationRegistrationCommandHandler, AuthenticationRegistrationCommandHandler>();
        services.AddScoped<IAuthenticationRegistrationRequestHandler, AuthenticationRegistrationRequestHandler>();
        services.AddScoped<IContextAuthenticationIdentityCommandHandler, ContextAuthenticationIdentityCommandHandler>();
        services.AddScoped<IContextAuthenticationRegistrationCommandHandler, ContextAuthenticationRegistrationCommandHandler>();

        return services;
    }
}