using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IContextAuthenticationIdentityCommandHandler
{
    Task<ClaimsPrincipal> AuthAsync(ContextAuthenticationIdentityAuthCommand command);

    void Verify();
}