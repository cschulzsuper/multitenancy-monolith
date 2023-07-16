using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Events;

internal sealed class NamedChannelDictionary<T>
{
    private readonly ConcurrentDictionary<string, NamedChannel<T>> _channels;

    private readonly ConcurrentQueue<NamedChannel<T>> _newChannels;

    private TaskCompletionSource _waitNew;

    public NamedChannelDictionary()
    {
        _channels = new ConcurrentDictionary<string, NamedChannel<T>>();
        _newChannels = new ConcurrentQueue<NamedChannel<T>>();
        _waitNew = new TaskCompletionSource();
    }

    public async Task<NamedChannel<T>> WaitNewAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (true) { 
                var dequeued = _newChannels.TryDequeue(out var channel);
                if (dequeued)
                {
                    return channel!;
                }

                using var localCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                localCancellationToken.Token.Register(_waitNew.SetCanceled);

                await _waitNew.Task;
            }
        }
        finally
        {
            _waitNew = new TaskCompletionSource();
        }
    }

    public async Task CompleteAllAsync(int timeout)
    {
        var copy = _channels.ToDictionary();

        foreach (var channel in copy)
        {
            channel.Value.ChannelWriter.Complete();
        }

        await Task.WhenAll(
            copy.Select(async namedChannel =>
            {
                await namedChannel.Value.ChannelReader.Completion
                    .WaitAsync(TimeSpan.FromMilliseconds(timeout))
                    .ConfigureAwait(false);
            }));
    }

    public NamedChannel<T> GetOrCreate(string name)
    {
        var channel = _channels.GetOrAdd(name, _ =>
        {
            var channel = Channel.CreateUnbounded<T>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = true
                });

            var namedChannel = new NamedChannel<T>
            {
                Name = name,
                Channel = channel
            };

            _newChannels.Enqueue(namedChannel);
            _waitNew.TrySetResult();

            return namedChannel;
        });

        return channel;
    }
}