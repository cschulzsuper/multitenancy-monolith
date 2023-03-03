using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerUserDependentBookmarkRequestHandler
{
    IAsyncEnumerable<TickerUserDependentBookmarkResponse> GetAll(string? query, int? skip, int? take);

    Task<TickerUserDependentBookmarkResponse> InsertAsync(TickerUserDependentBookmarkRequest request);

    Task DeleteAsync(long tickerMessage);
}
