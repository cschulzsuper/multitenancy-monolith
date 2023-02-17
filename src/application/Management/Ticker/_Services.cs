using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddTickerManagement(this IServiceCollection services)
    {
        services.AddScoped<ITickerMessageManager, TickerMessageManager>();
        services.AddScoped<ITickerUserManager, TickerUserManager>();
        services.AddScoped<ITickerUserVerificationManager, TickerUserVerificationManager>();

        return services;
    }
}