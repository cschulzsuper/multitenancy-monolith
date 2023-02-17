using ChristianSchulz.MultitenancyMonolith.Application.Authorization.Commands;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

public interface IMemberCommandHandler
{
    ValueTask<ClaimsIdentity> AuthAsync(MemberAuthCommand command);
    void Verify();
}