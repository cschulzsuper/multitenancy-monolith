using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Events;

public interface IEventSubscriptions
{
    void Map<THandler>(string @event, Func<THandler, EventSubscriptionInvocationContext, Task> subscription)
        where THandler : class;
}