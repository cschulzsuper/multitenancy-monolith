using ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal sealed class ContextAccountRegistrationCommandHandler : IContextAccountRegistrationCommandHandler
{
    private readonly IAccountRegistrationManager _accountRegistrationManager;
    private readonly IAccountGroupManager _accountGroupManager;
    private readonly IEventStorage _eventStorage;
    private readonly ClaimsPrincipal _user;

    public ContextAccountRegistrationCommandHandler(
        IAccountRegistrationManager accountRegistrationManager,
        IAccountGroupManager accountGroupManager,
        IEventStorage eventStorage,
        ClaimsPrincipal user)
    {
        _accountRegistrationManager = accountRegistrationManager;
        _accountGroupManager = accountGroupManager;
        _eventStorage = eventStorage;
        _user = user;
    }

    public async Task ConfirmAsync(ContextAccountRegistrationConfirmCommand command)
    {
        var updateAction = (AccountRegistration @object) =>
        {
            var contextAuthenticationIdentity = _user.GetClaim("identity");

            if (@object.AuthenticationIdentity != contextAuthenticationIdentity)
            {
                TransportException.ThrowSecurityViolation($"Authentication identity {contextAuthenticationIdentity} can not confirm account registration for account group '{command.AccountGroup}'");
            }

            switch (@object.ProcessState)
            {
                case AccountRegistrationProcessStates.New:
                    if (@object.ProcessToken != command.ProcessToken)
                    {
                        TransportException.ThrowSecurityViolation($"Process token of account registration for account group '{command.AccountGroup}' is not correct");
                    }

                    @object.ProcessState = AccountRegistrationProcessStates.Approved;

                    _eventStorage.Add("account-registration-confirmed", @object.Snowflake);
                    break;

                case AccountRegistrationProcessStates.Confirmed:
                    TransportException.ThrowProcessViolation($"Process state of account registration for account group '{command.AccountGroup}' is already confirmed");
                    break;

                case AccountRegistrationProcessStates.Approved:
                    TransportException.ThrowProcessViolation($"Process state of account registration for account group '{command.AccountGroup}' is approved");
                    break;

                case AccountRegistrationProcessStates.Completed:
                    TransportException.ThrowProcessViolation($"Process state of account registration for account group '{command.AccountGroup}' is completed");
                    break;

                default:
                    TransportException.ThrowSecurityViolation($"Secret state of account registration '{command.ProcessToken}' has unexpected value");
                    break;
            }
        };

        await _accountRegistrationManager.UpdateAsync(command.AccountGroup, updateAction);
    }

    public async Task RegisterAsync(ContextAccountRegistrationRegisterCommand command)
    {
        var exists = await _accountGroupManager.ExistsAsync(command.AccountGroup);
        if (exists)
        {
            TransportException.ThrowConflict<AccountGroup>(command.AccountGroup);
        }

        var @object = new AccountRegistration
        {
            AccountGroup = command.AccountGroup,
            AccountMember = command.AccountMember,
            MailAddress = command.MailAddress,
            AuthenticationIdentity = _user.GetClaim("identity"),
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid()
        };

        await _accountRegistrationManager.InsertAsync(@object);
    }
}