using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public static class TickerTransportServices
{
    public static IServiceCollection AddTickerTransport(this IServiceCollection services)
    {
        services.AddScoped<ITickerMessageRequestHandler, TickerMessageRequestHandler>();
        services.AddScoped<ITickerUserCommandHandler, TickerUserCommandHandler>();

        return services;
    }
}