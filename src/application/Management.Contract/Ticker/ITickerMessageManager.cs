using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerMessageManager
{
    ValueTask<TickerMessage> GetAsync(long snowflake);

    IAsyncEnumerable<TickerMessage> GetAsyncEnumerable(Func<IQueryable<TickerMessage>, IQueryable<TickerMessage>> query);

    ValueTask InsertAsync(TickerMessage tickerMessage);

    ValueTask UpdateAsync(long snowflake, Action<TickerMessage> action);

    ValueTask DeleteAsync(long snowflake);
}