using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChristianSchulz.MultitenancyMonolith.Events;

internal sealed class EventSubscriptions : IEventSubscriptions
{
    private readonly ILogger<EventSubscriptions> _logger;
    private readonly Dictionary<string, Func<IServiceProvider, long, Task>> _subscriptions;

    public EventSubscriptions(ILogger<EventSubscriptions> logger)
    {
        _logger = logger;
        _subscriptions = new Dictionary<string, Func<IServiceProvider, long, Task>>();
    }

    public void Map<THandler>(string eventName, Func<THandler, long, Task> subscription)
        where THandler : class
    {
        _subscriptions.Add(eventName,
            (services,snowflake) => ActionAsync(services, snowflake, subscription));
    }

    public async Task InvokeAsync(string @event, IServiceProvider services, long snowflake)
    {
        var found = _subscriptions.TryGetValue(@event, out var subscription);
        
        if(!found)
        {
            _logger.LogInformation("Event '{event}' subscription for '{snowflake}' not found", @event, snowflake);
            return;
        }

        _logger.LogInformation("Event '{event}' subscription for '{snowflake}' has been invoked", @event, snowflake);

        await subscription!(services, snowflake);
    }

    private async Task ActionAsync<THandler>(IServiceProvider services, long snowflake, Func<THandler, long, Task> subscription)
        where THandler : class
    {
        var handler = services.GetRequiredService<THandler>();

        await subscription(handler, snowflake);
    }
}