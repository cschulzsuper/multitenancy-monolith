using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddAdmissionManagement(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationIdentityManager, AuthenticationIdentityManager>();
        services.AddScoped<IAuthenticationIdentityAuthenticationMethodManager, AuthenticationIdentityAuthenticationMethodManager>();
        services.AddScoped<IAuthenticationRegistrationManager, AuthenticationRegistrationManager>();

        return services;
    }
}