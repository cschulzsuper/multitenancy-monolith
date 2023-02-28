using ChristianSchulz.MultitenancyMonolith.Application.Ticker;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddTickerOrchestration(this IServiceCollection services)
    {
        services.AddScoped<ITickerMessageBookmarker, TickerMessageBookmarker>();

        return services;
    }
}