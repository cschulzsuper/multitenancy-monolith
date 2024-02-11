using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Caching;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        services.AddScoped<IByteCacheFactory, ByteCacheFactory>();

        return services;
    }
}