using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Shared.EventBus;

internal sealed class EventSubscriptions : IEventSubscriptions
{
    private readonly IServiceProvider _services;
    private readonly Dictionary<string, Func<long, Task>> _subscriptions;

    public EventSubscriptions(IServiceProvider services)
    {
        _services = services;
        _subscriptions = new Dictionary<string, Func<long, Task>>();
    }

    public void Map<THandler>(string @event, Func<THandler, long, Task> subscription)
        where THandler : class
    {
        var eventSubscription = (long snowflake) => ActionAsync(snowflake, subscription);

        _subscriptions.Add(@event, eventSubscription);
    }

    private async Task ActionAsync<THandler>(long snowflake, Func<THandler, long, Task> subscription)
        where THandler : class
    {
        await using var scope = _services.CreateAsyncScope();

        var handler = scope.ServiceProvider.GetRequiredService<THandler>();

        await subscription.Invoke(handler, snowflake);
    }


    public bool TryGet(string @event, [MaybeNullWhen(false)] out Func<long, Task> subscription)
        => _subscriptions.TryGetValue(@event, out subscription);
}
