using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerMessageRequestHandler
{
    ValueTask<TickerMessageResponse> GetAsync(long tickerMessage);

    IAsyncEnumerable<TickerMessageResponse> GetAll(string query, int skip, int take);

    ValueTask<TickerMessageResponse> InsertAsync(TickerMessageRequest request);

    ValueTask UpdateAsync(long tickerMessage, TickerMessageRequest request);

    ValueTask DeleteAsync(long tickerMessage);
}