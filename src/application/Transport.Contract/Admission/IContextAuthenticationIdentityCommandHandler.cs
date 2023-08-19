using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IContextAuthenticationIdentityCommandHandler
{
    Task<object> AuthAsync(ContextAuthenticationIdentityAuthCommand command);

    Task VerifyAsync();
}