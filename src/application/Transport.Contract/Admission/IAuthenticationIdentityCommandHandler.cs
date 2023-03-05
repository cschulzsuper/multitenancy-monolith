using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IAuthenticationIdentityCommandHandler
{
    Task<ClaimsIdentity> AuthAsync(AuthenticationIdentityAuthCommand command);

    void Verify();
}