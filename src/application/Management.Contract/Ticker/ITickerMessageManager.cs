using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerMessageManager
{
    Task<TickerMessage> GetAsync(long tickerMessage);

    IAsyncEnumerable<TickerMessage> GetAsyncEnumerable(Func<IQueryable<TickerMessage>, IQueryable<TickerMessage>> query);

    Task InsertAsync(TickerMessage @object);

    Task UpdateAsync(long tickerMessage, Action<TickerMessage> action);

    Task DeleteAsync(long tickerMessage);
}