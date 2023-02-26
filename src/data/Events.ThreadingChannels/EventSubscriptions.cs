using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Events;

internal sealed class EventSubscriptions : IEventSubscriptions
{
    private readonly IServiceProvider _services;
    private readonly Dictionary<string, Func<EventValue, Task>> _subscriptions;

    public EventSubscriptions(IServiceProvider services)
    {
        _services = services;
        _subscriptions = new Dictionary<string, Func<EventValue, Task>>();
    }

    public void Map<THandler>(string eventName, Func<THandler, long, Task> subscription)
        where THandler : class
    {
        _subscriptions.Add(eventName,
            @event => ActionAsync(@event, subscription));
    }

    private async Task ActionAsync<THandler>(EventValue @event, Func<THandler, long, Task> subscription)
        where THandler : class
    {
        await using var scope = _services.CreateAsyncScope();

        var options = scope.ServiceProvider.GetRequiredService<EventsOptions>();

        options.SubscriptionInvocationSetup(scope.ServiceProvider, @event.Scope);

        var handler = scope.ServiceProvider.GetRequiredService<THandler>();

        await subscription(handler, @event.Snowflake);
    }


    public bool TryGet(string @event, [MaybeNullWhen(false)] out Func<EventValue, Task> subscription)
        => _subscriptions.TryGetValue(@event, out subscription);
}