using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class ContextTickerUserBookmarkRequestHandler : IContextTickerUserBookmarkRequestHandler
{
    private readonly ITickerBookmarkManager _tickerBookmarkManager;
    private readonly ClaimsPrincipal _user;

    public ContextTickerUserBookmarkRequestHandler(
        ITickerBookmarkManager tickerBookmarkManager,
        ClaimsPrincipal user)
    {
        _tickerBookmarkManager = tickerBookmarkManager;
        _user = user;
    }

    public async IAsyncEnumerable<ContextTickerUserBookmarkResponse> GetAll(string? query, int? skip, int? take)
    {
        var objects = _tickerBookmarkManager.GetAsyncEnumerable(
            query =>
            {
                query = query.Where(@object => @object.TickerUser == _user.GetClaim("mail"));

                query = skip != null ? query.Skip(skip.Value) : query;
                query = take != null ? query.Take(take.Value) : query;

                return query;
            });

        await foreach (var @object in objects)
        {
            var response = new ContextTickerUserBookmarkResponse
            {
                TickerMessage = @object.TickerMessage,
                Updated = @object.Updated
            };

            yield return response;
        }
    }

    public async Task<ContextTickerUserBookmarkResponse> InsertAsync(ContextTickerUserBookmarkRequest request)
    {
        var @object = new TickerBookmark
        {
            TickerMessage = request.TickerMessage,
            TickerUser = _user.GetClaim("mail"),
            Updated = false
        };

        await _tickerBookmarkManager.InsertAsync(@object);

        var response = new ContextTickerUserBookmarkResponse
        {
            TickerMessage = @object.TickerMessage,
            Updated = @object.Updated
        };

        return response;
    }

    public async Task DeleteAsync(long tickerMessage)
        => await _tickerBookmarkManager.DeleteAsync(_user.GetClaim("mail"), tickerMessage);
}