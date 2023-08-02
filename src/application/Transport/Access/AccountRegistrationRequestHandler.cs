using ChristianSchulz.MultitenancyMonolith.Application.Access.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Access.Responses;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal sealed class AccountRegistrationRequestHandler : IAccountRegistrationRequestHandler
{
    private readonly IAccountRegistrationManager _accountRegistrationManager;

    public AccountRegistrationRequestHandler(IAccountRegistrationManager accountRegistrationManager)
    {
        _accountRegistrationManager = accountRegistrationManager;
    }

    public async Task<AccountRegistrationResponse> GetAsync(long accountRegistration)
    {
        var @object = await _accountRegistrationManager.GetAsync(accountRegistration);

        var response = new AccountRegistrationResponse
        {
            Snowflake = @object.Snowflake,
            AccountGroup = @object.AccountGroup,
            AccountMember = @object.AccountMember,
            AuthenticationIdentity = @object.AuthenticationIdentity,
            MailAddress = @object.MailAddress,
            ProcessState = @object.ProcessState
        };

        return response;
    }

    public IQueryable<AccountRegistrationResponse> GetAll()
    {
        var objects = _accountRegistrationManager.GetQueryable();

        var response = objects.Select(@object =>
            new AccountRegistrationResponse
            {
                Snowflake = @object.Snowflake,
                AccountGroup = @object.AccountGroup,
                AccountMember = @object.AccountMember,
                AuthenticationIdentity = @object.AuthenticationIdentity,
                MailAddress = @object.MailAddress,
                ProcessState = @object.ProcessState
            });

        return response;
    }

    public async Task<AccountRegistrationResponse> InsertAsync(AccountRegistrationRequest request)
    {
        var @object = new AccountRegistration
        {
            AccountGroup = request.AccountGroup,
            AccountMember = request.AccountMember,
            AuthenticationIdentity = request.AuthenticationIdentity,
            MailAddress = request.MailAddress,
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid()
        };

        await _accountRegistrationManager.InsertAsync(@object);

        var response = new AccountRegistrationResponse
        {
            Snowflake = @object.Snowflake,
            AccountGroup = @object.AccountGroup,
            AccountMember = @object.AccountMember,
            AuthenticationIdentity = @object.AuthenticationIdentity,
            MailAddress = @object.MailAddress,
            ProcessState = @object.ProcessState
        };

        return response;
    }

    public async Task UpdateAsync(long accountRegistration, AccountRegistrationRequest request)
        => await _accountRegistrationManager.UpdateAsync(accountRegistration,
            @object =>
            {
                @object.AccountGroup = request.AccountGroup;
                @object.AccountMember = request.AccountMember;
                @object.AuthenticationIdentity = request.AuthenticationIdentity;
                @object.MailAddress = request.MailAddress;
            });

    public async Task DeleteAsync(long accountRegistration)
        => await _accountRegistrationManager.DeleteAsync(accountRegistration);
}