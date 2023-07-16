using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class AuthenticationRegistrationCommandHandler : IAuthenticationRegistrationCommandHandler
{
    private readonly IAuthenticationRegistrationManager _authenticationRegistrationManager;
    private readonly IEventStorage _eventStorage;

    public AuthenticationRegistrationCommandHandler(
        IAuthenticationRegistrationManager authenticationRegistrationManager,
        IEventStorage eventStorage)
    {
        _authenticationRegistrationManager = authenticationRegistrationManager;
        _eventStorage = eventStorage;
    }

    public async Task ApproveAsync(long authenticationRegistration)
    {
        var updateAction = (AuthenticationRegistration @object) =>
        {
            switch (@object.ProcessState)
            {
                case AuthenticationRegistrationProcessStates.New:
                    TransportException.ThrowProcessViolation($"Process state of authentication registration '{authenticationRegistration}' is new");
                    break;

                case AuthenticationRegistrationProcessStates.Confirmed:
                    @object.ProcessState = AuthenticationRegistrationProcessStates.Approved;

                    _eventStorage.Add("authentication-registration-approved", @object.Snowflake);
                    break;

                case AuthenticationRegistrationProcessStates.Approved:
                    TransportException.ThrowProcessViolation($"Process state of authentication registration '{authenticationRegistration}' is already approved");
                    break;

                case AuthenticationRegistrationProcessStates.Completed:
                    TransportException.ThrowProcessViolation($"Process state of authentication registration '{authenticationRegistration}' is completed");
                    break;

                default:
                    TransportException.ThrowSecurityViolation($"Secret state of authentication registration '{authenticationRegistration}' has unexpected value");
                    break;
            }
        };

        await _authenticationRegistrationManager.UpdateAsync(authenticationRegistration, updateAction);
    }
}
