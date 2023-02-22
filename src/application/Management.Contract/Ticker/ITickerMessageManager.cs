using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerMessageManager
{
    Task<TickerMessage> GetAsync(long snowflake);

    IAsyncEnumerable<TickerMessage> GetAsyncEnumerable(Func<IQueryable<TickerMessage>, IQueryable<TickerMessage>> query);

    Task InsertAsync(TickerMessage tickerMessage);

    Task UpdateAsync(long snowflake, Action<TickerMessage> action);

    Task DeleteAsync(long snowflake);
}