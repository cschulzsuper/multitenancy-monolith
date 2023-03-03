using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerMessageBookmarker
{
    Task BookmarkAsync(long tickerMessage);

    Task RefreshAsync(long tickerMessage);

    Task PurgeAsync(long tickerMessage);
}