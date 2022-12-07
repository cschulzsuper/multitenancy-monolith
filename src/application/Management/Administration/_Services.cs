using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public static class _Services
{
    public static IServiceCollection AddAdministrationManagement(this IServiceCollection services)
    {
        services.AddScoped<IMemberManager, MemberManager>();
        services.AddScoped<IMembershipManager, MembershipManager>();
        services.AddScoped<IMembershipVerficationManager, MembershipVerficationManager>();

        return services;
    }
}
