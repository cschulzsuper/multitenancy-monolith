using ChristianSchulz.MultitenancyMonolith.Events;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public static class TickerMessageEvents
{
    public static IEventSubscriptions MapTickerMessageEvents(this IEventSubscriptions subscriptions)
    {
        subscriptions.Map("ticker-message-inserted", Inserted);

        return subscriptions;
    }

    private static Func<ITickerMessageBookmarker, long, Task> Inserted =>
        (eventHandler, tickerMessage)
            => eventHandler.BookmarkAsync(tickerMessage);
}
