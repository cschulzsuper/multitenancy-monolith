using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerUserManager
{
    ValueTask<TickerUser> GetAsync(string ticketUser);

    ValueTask<TickerUser?> GetOrDefaultAsync(string tickerUser);

    ValueTask InsertAsync(TickerUser @object);

    ValueTask UpdateOrDefaultAsync(string tickerUser, Action<TickerUser> action, Action @default);
}
