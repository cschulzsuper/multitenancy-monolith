using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace ChristianSchulz.MultitenancyMonolith.Shared.EventBus;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        services.AddScoped<IEventStorage, EventStorage>();

        services.AddSingleton(Channel.CreateUnbounded<EventValue>(
            new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = true
            }));

        services.AddSingleton(services => services.GetRequiredService<Channel<EventValue>>().Reader);
        services.AddSingleton(services => services.GetRequiredService<Channel<EventValue>>().Writer);

        services.AddSingleton<IEventPublisher, EventPublisher>();
        services.AddSingleton<IEventSubscriptions, EventSubscriptions>();

        services.AddHostedService<EventService>();

        return services;
    }
}