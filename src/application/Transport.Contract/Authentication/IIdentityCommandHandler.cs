using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Commands;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityCommandHandler
{
    Task<ClaimsIdentity> AuthAsync(IdentityAuthCommand command);

    void Verify();
}