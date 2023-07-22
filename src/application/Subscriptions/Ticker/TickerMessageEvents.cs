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
        (bookmarker, tickerMessage)
            => bookmarker.BookmarkAsync(tickerMessage);

    private static Func<ITickerMessageBookmarker, long, Task> Updated =>
        (bookmarker, tickerMessage)
            => bookmarker.RefreshAsync(tickerMessage);

    private static Func<ITickerMessageBookmarker, long, Task> Deleted =>
        (bookmarker, tickerMessage)
            => bookmarker.PurgeAsync(tickerMessage);
}