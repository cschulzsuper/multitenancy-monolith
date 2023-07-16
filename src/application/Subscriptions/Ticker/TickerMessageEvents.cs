using ChristianSchulz.MultitenancyMonolith.Events;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public static class TickerMessageEvents
{
    public static IEventSubscriptions MapTickerMessageSubscriptions(this IEventSubscriptions subscriptions)
    {
        subscriptions.Map("ticker-message-inserted", Inserted);
        subscriptions.Map("ticker-message-updated", Updated);
        subscriptions.Map("ticker-message-deleted", Deleted);

        return subscriptions;
    }

    private static Func<ITickerMessageBookmarker, long, Task> Inserted =>
        (eventHandler, tickerMessage)
            => eventHandler.BookmarkAsync(tickerMessage);

    private static Func<ITickerMessageBookmarker, long, Task> Updated =>
        (eventHandler, tickerMessage)
            => eventHandler.RefreshAsync(tickerMessage);

    private static Func<ITickerMessageBookmarker, long, Task> Deleted =>
        (eventHandler, tickerMessage)
            => eventHandler.PurgeAsync(tickerMessage);
}
