using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerUserManager
{
    ValueTask<TickerUser?> GetOrDefaultAsync(string ticketUser);

    ValueTask<bool> ExistsAsync(string tickerUser, string secret);

    ValueTask InsertAsync(TickerUser @object);
}
