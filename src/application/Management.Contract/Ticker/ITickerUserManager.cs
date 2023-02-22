using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerUserManager
{
    Task<TickerUser> GetAsync(string ticketUser);

    Task<TickerUser?> GetOrDefaultAsync(string tickerUser);

    Task InsertAsync(TickerUser @object);

    Task UpdateAsync(string tickerUser, Action<TickerUser> action, Action @default);
}
