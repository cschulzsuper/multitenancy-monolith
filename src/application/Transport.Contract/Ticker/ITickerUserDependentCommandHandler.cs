using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerUserDependentCommandHandler
{
    Task<ClaimsIdentity> AuthAsync(TickerUserDependentAuthCommand command);

    Task<ClaimsIdentity> ConfirmAsync(TickerUserDependentConfirmCommand command);

    Task PostAsync(TickerUserDependentPostCommand command);

    void Verify();
}
