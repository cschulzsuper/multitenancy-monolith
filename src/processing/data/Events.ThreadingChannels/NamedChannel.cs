using System.Threading.Channels;

namespace ChristianSchulz.MultitenancyMonolith.Events;

internal sealed class NamedChannel<T>
{
    public required string Name { get; init; }

    public required Channel<T> Channel { get; init; }

    public ChannelReader<T> ChannelReader => Channel.Reader;

    public ChannelWriter<T> ChannelWriter => Channel.Writer;
}