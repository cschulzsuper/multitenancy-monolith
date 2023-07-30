using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface IContextTickerUserCommandHandler
{
    Task<ClaimsPrincipal> AuthAsync(ContextTickerUserAuthCommand command);

    Task ConfirmAsync(ContextTickerUserConfirmCommand command);

    Task PostAsync(ContextTickerUserPostCommand command);

    void Verify();
}