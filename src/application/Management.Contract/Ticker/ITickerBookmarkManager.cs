using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerBookmarkManager
{
    Task<TickerBookmark> GetAsync(long tickerBookmark);

    IAsyncEnumerable<TickerBookmark> GetAsyncEnumerable(Func<IQueryable<TickerBookmark>, IQueryable<TickerBookmark>> query);

    Task InsertAsync(TickerBookmark @object);

    Task UpdateAsync(long tickerBookmark, Action<TickerBookmark> action);

    Task UpdateAsync(string tickerUser, long tickerMessage, Action<TickerBookmark> action);

    Task UpdateManyAsync(Expression<Func<TickerBookmark, bool>> predicate, Action<TickerBookmark> action);

    Task DeleteAsync(long tickerBookmark);

    Task DeleteAsync(string tickerUser, long tickerMessage);

    Task DeleteManyAsync(Expression<Func<TickerBookmark, bool>> predicate);
}