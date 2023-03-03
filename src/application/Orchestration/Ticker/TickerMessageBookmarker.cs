﻿using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerMessageBookmarker : ITickerMessageBookmarker
{
    private readonly ITickerMessageManager _tickerMessageManager;
    private readonly ITickerBookmarkManager _tickerBookmarkManager;

    public TickerMessageBookmarker(
        ITickerMessageManager tickerMessageManager,
        ITickerBookmarkManager tickerBookmarkManager)
    {
        _tickerMessageManager = tickerMessageManager;
        _tickerBookmarkManager = tickerBookmarkManager;
    }

    public async Task BookmarkAsync(long tickerMessage)
    {
        var messageObject = await _tickerMessageManager.GetAsync(tickerMessage);

        var bookmarkObject = new TickerBookmark
        {
            TickerMessage = messageObject.Snowflake,
            TickerUser = messageObject.TickerUser,
            Updated = true
        };

        await _tickerBookmarkManager.InsertAsync(bookmarkObject);
    }
}