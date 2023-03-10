using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class ContextAuthenticationRegistrationCommandHandler : IContextAuthenticationRegistrationCommandHandler
{
    private readonly IAuthenticationRegistrationManager _authenticationRegistrationManager;
    private readonly IAuthenticationIdentityManager _authenticationIdentityManager;

    public ContextAuthenticationRegistrationCommandHandler(
        IAuthenticationRegistrationManager authenticationRegistrationManager,
        IAuthenticationIdentityManager authenticationIdentityManager)
    {
        _authenticationRegistrationManager = authenticationRegistrationManager;
        _authenticationIdentityManager = authenticationIdentityManager;
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
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            Secret = command.Secret,
        };

        await _authenticationRegistrationManager.InsertAsync(@object);
    }
}