using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddAccessManagement(this IServiceCollection services)
    {
        services.AddScoped<IAccountGroupManager, AccountGroupManager>();
        services.AddScoped<IAccountMemberManager, AccountMemberManager>();
        services.AddScoped<IAccountMemberVerificationManager, AccountMemberVerificationManager>();
        services.AddScoped<IAccountRegistrationManager, AccountRegistrationManager>();

        return services;
    }
}