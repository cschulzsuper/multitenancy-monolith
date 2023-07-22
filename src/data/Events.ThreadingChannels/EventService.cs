using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChristianSchulz.MultitenancyMonolith.Events;

internal sealed class EventService : BackgroundService
{
    private readonly ILogger<EventService> _logger;
    private readonly NamedChannelDictionary<EventValue> _channels;
    private readonly TaskCollection _channelListeners;
    private readonly IServiceProvider _services;
    private readonly EventsOptions _options;
    private readonly EventSubscriptions _subscriptions;

    public EventService(
        ILogger<EventService> logger,
        NamedChannelDictionary<EventValue> channels,
        TaskCollection channelListeners,
        IEventSubscriptions subscriptions,
        IServiceProvider services,
        EventsOptions options)
    {
        _logger = logger;
        _channels = channels;
        _channelListeners = channelListeners;
        _services = services;
        _options = options;

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

                try
                {
                    await using var scope = _services.CreateAsyncScope();

                    await _options.BeforeSubscriptionInvocation(scope.ServiceProvider, channel.Name);

                    await _subscriptions.InvokeAsync(@event!.Event, scope.ServiceProvider, @event.Snowflake);
                    _logger.LogInformation("Event '{event}' subscription for '{snowflake}' has been invoked", @event.Event, @event.Snowflake);

                    _ = await channel.ChannelReader
                        .ReadAsync(cancellationToken)
                        .ConfigureAwait(false);

                    await _options.AfterSubscriptionInvocation(scope.ServiceProvider, channel.Name);

                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    _logger.LogError(exception, "Error while execution event '{event}' subscription for '{snowflake}' ", @event!.Event, @event.Snowflake);
                }
            }
        }

        var channelListener = Task.Run(ChannelListener, cancellationToken);

        _channelListeners.Add(channelListener);
    }
}