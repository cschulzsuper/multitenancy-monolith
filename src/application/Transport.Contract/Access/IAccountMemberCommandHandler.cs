using ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public interface IAccountMemberCommandHandler
{
    Task<ClaimsIdentity> AuthAsync(AccountMemberAuthCommand command);
    void Verify();
}