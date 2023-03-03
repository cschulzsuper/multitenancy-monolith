using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerUserCommandHandler
{
    Task ResetAsync(long tickerUser);
}