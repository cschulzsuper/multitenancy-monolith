using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Events;

internal sealed class EventPublisher : IEventPublisher
{
    private readonly NamedChannel<EventValue> _channel;

    public EventPublisher(
        NamedChannelDictionary<EventValue> channels,
        EventsOptions options,
        IServiceProvider services)
    {
        var channelName = options.ChannelNameResolver(services);

        _channel = channels.GetOrCreate(channelName);
    }

    public async Task PublishAsync(string @event, long snowflake)
    {
        var eventValue = new EventValue
        {
            Event = @event,
            Scope = _channel.Name,
            Snowflake = snowflake
        };

        await _channel.ChannelWriter.WriteAsync(eventValue);
    }
}