using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerUserManager
{
    Task<TickerUser> GetAsync(long ticketUser);

    IAsyncEnumerable<TickerUser> GetAsyncEnumerable(Func<IQueryable<TickerUser>, IQueryable<TickerUser>> query);

    Task InsertAsync(TickerUser @object);

    Task UpdateAsync(long tickerUser, Action<TickerUser> action);

    Task UpdateAsync(string tickerUser, Action<TickerUser> action, Action @default);

    Task DeleteAsync(long tickerUser);
}