using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerMessageRequestHandler
{
    Task<TickerMessageResponse> GetAsync(long tickerMessage);

    IAsyncEnumerable<TickerMessageResponse> GetAll(string? query, int? skip, int? take);

    Task<TickerMessageResponse> InsertAsync(TickerMessageRequest request);

    Task UpdateAsync(long tickerMessage, TickerMessageRequest request);

    Task DeleteAsync(long tickerMessage);
}