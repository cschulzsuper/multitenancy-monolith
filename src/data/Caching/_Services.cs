using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Caching;

public static class _Services
{
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        services.AddScoped<IByteCacheFactory, ByteCacheFactory>();

        return services;
    }
}
