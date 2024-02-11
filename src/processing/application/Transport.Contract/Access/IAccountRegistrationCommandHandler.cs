using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public interface IAccountRegistrationCommandHandler
{
    Task ApproveAsync(long authenticationRegistration);
}