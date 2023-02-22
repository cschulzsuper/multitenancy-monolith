using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerUserCommandHandler
{
    Task<ClaimsIdentity> AuthAsync(TickerUserAuthCommand command);

    Task<ClaimsIdentity> ConfirmAsync(TickerUserConfirmCommand command);

    Task PostAsync(TickerUserPostCommand command);

    void Verify();
}
