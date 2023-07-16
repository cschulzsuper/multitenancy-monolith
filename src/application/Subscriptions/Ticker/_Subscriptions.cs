using ChristianSchulz.MultitenancyMonolith.Events;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Subscriptions
{
    public static IEventSubscriptions MapTickerSubscriptions(this IEventSubscriptions subscriptions)
    {
        subscriptions.MapTickerMessageSubscriptions();

        return subscriptions;
    }
}
