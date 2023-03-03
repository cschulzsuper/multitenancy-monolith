using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface IContextTickerUserBookmarkRequestHandler
{
    IAsyncEnumerable<ContextTickerUserBookmarkResponse> GetAll(string? query, int? skip, int? take);

    Task<ContextTickerUserBookmarkResponse> InsertAsync(ContextTickerUserBookmarkRequest request);

    Task DeleteAsync(long tickerMessage);
}
