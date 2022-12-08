using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public static partial class _Services
{
    public static IServiceCollection AddData(this IServiceCollection services)
    {
        services.AddScoped(MemberRepositoryFactory);
        services.AddScoped(MembershipRepositoryFactory);
        services.AddScoped(IdentityRepositoryFactory);

        return services;
    }

}
