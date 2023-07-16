using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class ContextTickerUserBookmarkCommandHandler : IContextTickerUserBookmarkCommandHandler
{
    private readonly ITickerBookmarkManager _tickerBookmarkManager;
    private readonly ClaimsPrincipal _user;

    public ContextTickerUserBookmarkCommandHandler(
        ITickerBookmarkManager tickerBookmarkManager,
        ClaimsPrincipal user)
    {
        _tickerBookmarkManager = tickerBookmarkManager;
        _user = user;
    }

    public async Task ConfirmAsync(long tickerMessage)
    {
        var updateAction = (TickerBookmark @object) =>
        {
            @object.Updated = false;
        };

        await _tickerBookmarkManager.UpdateAsync(_user.GetClaim("mail"), tickerMessage, updateAction);
    }
}