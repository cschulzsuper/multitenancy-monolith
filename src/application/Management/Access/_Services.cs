using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddAccessManagement(this IServiceCollection services)
    {
        services.AddScoped<IAccountGroupManager, AccountGroupManager>();
        services.AddScoped<IAccountMemberManager, AccountMemberManager>();
        services.AddScoped<IAccountRegistrationManager, AccountRegistrationManager>();

        return services;
    }
}