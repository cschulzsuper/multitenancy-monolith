using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IAuthenticationRegistrationCommandHandler
{
    Task ApproveAsync(long authenticationRegistration);
}