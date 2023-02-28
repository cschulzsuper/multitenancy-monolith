using ChristianSchulz.MultitenancyMonolith.Events;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Events
{
    public static IEventSubscriptions MapTickerEvents(this IEventSubscriptions subscriptions)
    {
        subscriptions.MapTickerMessageEvents();

        return subscriptions;
    }
}
