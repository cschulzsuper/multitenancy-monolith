using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public static class _Services
{
    public static IServiceCollection AddAdministrationTransport(this IServiceCollection services)
    {
        services.AddScoped<IMemberSignInRequestHandler, MemberSignInRequestHandler>();
        services.AddScoped<IMemberRequestHandler, MemberRequestHandler>();

        return services;
    }
}