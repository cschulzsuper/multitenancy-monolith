using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Events;

internal sealed class EventPublisher : IEventPublisher
{
    private readonly EventsOptions _options;

    private readonly NamedChannel<EventValue> _channel;
    public EventPublisher(
        NamedChannelDictionary<EventValue> channels,
        EventsOptions options,
        IServiceProvider services)
    {
        var channelName = options.ChannelNameResolver(services);

        _channel = channels.GetOrCreate(channelName);
        _options = options;
    }

    public async Task PublishAsync(string @event, long snowflake)
    {
        var eventValue = new EventValue
        {
            Event = @event,
            Snowflake = snowflake
        };

        _options.PublicationInterceptor(_channel.Name, @event, snowflake);

        await _channel.ChannelWriter.WriteAsync(eventValue);
    }
}