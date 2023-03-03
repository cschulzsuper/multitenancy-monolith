using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddTickerTransport(this IServiceCollection services)
    {
        services.AddScoped<IContextTickerUserCommandHandler, ContextTickerUserCommandHandler>();
        services.AddScoped<IContextTickerUserBookmarkRequestHandler, ContextTickerUserBookmarkRequestHandler>();
        services.AddScoped<ITickerMessageRequestHandler, TickerMessageRequestHandler>();
        services.AddScoped<ITickerUserCommandHandler, TickerUserCommandHandler>();
        services.AddScoped<ITickerUserRequestHandler, TickerUserRequestHandler>();

        return services;
    }
}