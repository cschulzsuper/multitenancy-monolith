using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IContextAuthenticationIdentityCommandHandler
{
    Task<ClaimsIdentity> AuthAsync(ContextAuthenticationIdentityAuthCommand command);

    void Verify();
}