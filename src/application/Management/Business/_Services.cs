using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddBusinessManagement(this IServiceCollection services)
    {
        services.AddScoped<IBusinessObjectManager, BusinessObjectManager>();

        return services;
    }
}