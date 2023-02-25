using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChristianSchulz.MultitenancyMonolith.Events;

internal sealed class EventService : BackgroundService
{
    private readonly ILogger<IEventSubscriptions> _logger;
    private readonly NamedChannelDictionary<EventValue> _channels;
    private readonly TaskCollection _channelListeners;
    private readonly EventSubscriptions _subscriptions;

    public EventService(
        ILogger<IEventSubscriptions> logger,
        NamedChannelDictionary<EventValue> channels,
        TaskCollection channelListeners,
        IEventSubscriptions subscriptions)
    {
        _logger = logger;
        _channels = channels;
        _channelListeners = channelListeners;

        _subscriptions = subscriptions as EventSubscriptions ??
                         throw new UnreachableException($"Parameter {subscriptions} (IEventSubscriptions) must be of type EventSubscriptions");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Yield();
                
                var channel = await _channels
                    .WaitNewAsync(stoppingToken);

                CreateChannelListener(channel, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error while waiting on new event channels");
            }
        }

        await _channelListeners
            .WaitAsync(stoppingToken);
    }

    private void CreateChannelListener(NamedChannel<EventValue> channel, CancellationToken cancellationToken)
    {
        async Task ChannelListener()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var completed = !await channel.ChannelReader
                    .WaitToReadAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (completed)
                {
                    break;
                }

                var peeked = channel.ChannelReader.TryPeek(out var @event);
                if (!peeked)
                {
                    continue;
                }

                var found = _subscriptions.TryGet(@event!.Event, out var subscription);
                if (!found)
                {
                    _logger.LogInformation("Event '{event}' subscription for '{snowflake}' not found", @event.Event, @event.Snowflake);

                    _ = await channel.ChannelReader
                        .ReadAsync(cancellationToken)
                        .ConfigureAwait(false);

                    continue;
                }

                try
                {
                    await subscription!
                        .Invoke(@event);

                    _logger.LogInformation("Event '{event}' subscription for '{snowflake}' has been invoked", @event.Event, @event.Snowflake);

                    _ = await channel.ChannelReader
                        .ReadAsync(cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    _logger.LogError(exception, "Error while execution event '{event}' subscription for '{snowflake}' ", @event.Event, @event.Snowflake);
                }
            }
        }

        var channelListener = Task.Run(ChannelListener, cancellationToken);

        _channelListeners.Add(channelListener);
    }
}