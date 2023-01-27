using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

public static class _Services
{
    public static IServiceCollection AddBusinessTransport(this IServiceCollection services)
    {
        services.AddScoped<IBusinessObjectRequestHandler, BusinessObjectRequestHandler>();

        return services;
    }
}