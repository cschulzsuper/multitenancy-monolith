using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddTickerTransport(this IServiceCollection services)
    {
        services.AddScoped<ITickerMessageRequestHandler, TickerMessageRequestHandler>();
        services.AddScoped<ITickerUserCommandHandler, TickerUserCommandHandler>();

        return services;
    }
}