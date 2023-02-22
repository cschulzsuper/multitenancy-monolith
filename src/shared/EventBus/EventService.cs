using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Channels;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ChristianSchulz.MultitenancyMonolith.Shared.EventBus
{
    internal sealed class EventService : BackgroundService
    {
        private readonly ILogger<IEventSubscriptions> _logger;
        private readonly ChannelReader<EventValue> _channelReader;
        private readonly EventSubscriptions _subscriptions;

        public EventService(
            ILogger<IEventSubscriptions> logger,
            ChannelReader<EventValue> channelReader,
            IEventSubscriptions subscriptions)
        {
            _logger = logger;
            _channelReader = channelReader;
            _subscriptions = subscriptions as EventSubscriptions ??
                throw new UnreachableException($"Parameter {subscriptions} (IEventSubscriptions) must be of type EventSubscriptions");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var @event = await _channelReader
                        .ReadAsync(stoppingToken)
                        .ConfigureAwait(false);

                    var found = _subscriptions.TryGet(@event.Event, out var subscription);
                    if(!found)
                    {
                        _logger.LogInformation("Event '{event}' subscription for '{snowflake}' not found", @event.Event, @event.Snowflake);
                        continue;
                    }

                    try
                    {
                        await subscription!
                            .Invoke(@event.Snowflake)
                            .ConfigureAwait(false);

                        _logger.LogInformation("Event '{event}' subscription for '{snowflake}' has been invoked", @event.Event, @event.Snowflake);
                    }
                    catch (Exception exception) when (exception is not OperationCanceledException)
                    {
                        _logger.LogError(exception, "Error while execution event '{event}' subscription for '{snowflake}' ", @event.Event, @event.Snowflake);
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error while iterating events ");
                }
            }
        }
    }
}
