using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerUserCommandHandler
{
    ValueTask<ClaimsIdentity> AuthAsync(TickerUserAuthCommand command);

    void Verify();
}