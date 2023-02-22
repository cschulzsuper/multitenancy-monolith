using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Shared.EventBus;

internal sealed class EventPublisher : IEventPublisher
{
    private readonly ChannelWriter<EventValue> _channelWriter;

    public EventPublisher(ChannelWriter<EventValue> channelWriter)
    {
        _channelWriter = channelWriter;
    }

    public async Task PublishAsync(string @event, long snowflake)
    {
        var eventValue = new EventValue
        {
            Event = @event,
            Snowflake = snowflake
        };

        await _channelWriter.WriteAsync(eventValue);
    }
}
