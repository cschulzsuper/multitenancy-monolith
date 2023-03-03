using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerUserRequestHandler
{
    Task<TickerUserResponse> GetAsync(long tickerUser);

    IAsyncEnumerable<TickerUserResponse> GetAll(string? query, int? skip, int? take);

    Task<TickerUserResponse> InsertAsync(TickerUserRequest request);

    Task UpdateAsync(long tickerUser, TickerUserRequest request);

    Task DeleteAsync(long tickerUser);
}