using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface IContextTickerUserBookmarkCommandHandler
{
    Task ConfirmAsync(long tickerBookmark);
}
