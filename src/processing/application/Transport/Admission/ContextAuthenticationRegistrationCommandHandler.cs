using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class ContextAuthenticationRegistrationCommandHandler : IContextAuthenticationRegistrationCommandHandler
{
    private readonly IAuthenticationRegistrationManager _authenticationRegistrationManager;
    private readonly IAuthenticationIdentityManager _authenticationIdentityManager;
    private readonly IEventStorage _eventStorage;

    public ContextAuthenticationRegistrationCommandHandler(
        IAuthenticationRegistrationManager authenticationRegistrationManager,
        IAuthenticationIdentityManager authenticationIdentityManager,
        IEventStorage eventStorage)
    {
        _authenticationRegistrationManager = authenticationRegistrationManager;
        _authenticationIdentityManager = authenticationIdentityManager;
        _eventStorage = eventStorage;
    }

    public async Task ConfirmAsync(ContextAuthenticationRegistrationConfirmCommand command)
    {
        var updateAction = (AuthenticationRegistration @object) =>
        {
            switch (@object.ProcessState)
            {
                case AuthenticationRegistrationProcessStates.New:

                    if (@object.Secret != command.Secret)
                    {
                        TransportException.ThrowSecurityViolation($"Secret of authentication registration for authentication identity '{command.AuthenticationIdentity}' is not correct");
                    }

                    if (@object.ProcessToken != command.ProcessToken)
                    {
                        TransportException.ThrowSecurityViolation($"Process token of authentication registration for authentication identity '{command.AuthenticationIdentity}' is not correct");
                    }

                    @object.ProcessState = AuthenticationRegistrationProcessStates.Approved;

                    _eventStorage.Add("authentication-registration-confirmed", @object.Snowflake);
                    break;

                case AuthenticationRegistrationProcessStates.Confirmed:
                    TransportException.ThrowProcessViolation($"Process state of authentication registration for authentication identity '{command.AuthenticationIdentity}' is already confirmed");
                    break;

                case AuthenticationRegistrationProcessStates.Approved:
                    TransportException.ThrowProcessViolation($"Process state of authentication registration for authentication identity '{command.AuthenticationIdentity}' is approved");
                    break;

                case AuthenticationRegistrationProcessStates.Completed:
                    TransportException.ThrowProcessViolation($"Process state of authentication registration for authentication identity '{command.AuthenticationIdentity}' is completed");
                    break;

                default:
                    TransportException.ThrowSecurityViolation($"Secret state of authentication registration '{command.ProcessToken}' has unexpected value");
                    break;
            }
        };

        await _authenticationRegistrationManager.UpdateAsync(command.AuthenticationIdentity, updateAction);
    }

    public async Task RegisterAsync(ContextAuthenticationRegistrationRegisterCommand command)
    {
        var exists = await _authenticationIdentityManager.ExistsAsync(command.AuthenticationIdentity);
        if (exists)
        {
            TransportException.ThrowConflict<AuthenticationIdentity>(command.AuthenticationIdentity);
        }

        var @object = new AuthenticationRegistration
        {
            AuthenticationIdentity = command.AuthenticationIdentity,
            MailAddress = command.MailAddress,
            ProcessState = AuthenticationRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            Secret = command.Secret,
        };

        await _authenticationRegistrationManager.InsertAsync(@object);
    }
}