using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Shared.EventBus;

public interface IEventSubscriptions
{
    void Map<THandler>(string @event, Func<THandler, long, Task> subscription)
        where THandler : class;
}
