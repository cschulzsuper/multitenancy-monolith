using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal sealed class AccountRegistrationCommandHandler : IAccountRegistrationCommandHandler
{
    private readonly IAccountRegistrationManager _accountRegistrationManager;
    private readonly IEventStorage _eventStorage;

    public AccountRegistrationCommandHandler(
        IAccountRegistrationManager accountRegistrationManager,
        IEventStorage eventStorage)
    {
        _accountRegistrationManager = accountRegistrationManager;
        _eventStorage = eventStorage;
    }

    public async Task ApproveAsync(long authenticationRegistration)
    {
        var updateAction = (AccountRegistration @object) =>
        {
            switch (@object.ProcessState)
            {
                case AccountRegistrationProcessStates.New:
                    TransportException.ThrowProcessViolation($"Process state of account registration '{authenticationRegistration}' is new");
                    break;

                case AccountRegistrationProcessStates.Confirmed:
                    @object.ProcessState = AccountRegistrationProcessStates.Approved;

                    _eventStorage.Add("account-registration-approved", @object.Snowflake);
                    break;

                case AccountRegistrationProcessStates.Approved:
                    TransportException.ThrowProcessViolation($"Process state of account registration '{authenticationRegistration}' is already approved");
                    break;

                case AccountRegistrationProcessStates.Completed:
                    TransportException.ThrowProcessViolation($"Process state of account registration '{authenticationRegistration}' is completed");
                    break;

                default:
                    TransportException.ThrowSecurityViolation($"Secret state of account registration '{authenticationRegistration}' has unexpected value");
                    break;
            }
        };

        await _accountRegistrationManager.UpdateAsync(authenticationRegistration, updateAction);
    }
}