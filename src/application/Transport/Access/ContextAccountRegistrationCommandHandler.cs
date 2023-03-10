using ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;
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
    private readonly ClaimsPrincipal _user;

    public ContextAccountRegistrationCommandHandler(
        IAccountRegistrationManager accountRegistrationManager,
        IAccountGroupManager accountGroupManager,
        ClaimsPrincipal user)
    {
        _accountRegistrationManager = accountRegistrationManager;
        _accountGroupManager = accountGroupManager;
        _user = user;
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