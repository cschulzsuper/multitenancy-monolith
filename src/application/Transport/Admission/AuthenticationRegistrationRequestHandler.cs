using System;
using System.Linq;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Responses;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class AuthenticationRegistrationRequestHandler : IAuthenticationRegistrationRequestHandler
{
    private readonly IAuthenticationRegistrationManager _authenticationRegistrationManager;

    public AuthenticationRegistrationRequestHandler(IAuthenticationRegistrationManager authenticationRegistrationManager)
    {
        _authenticationRegistrationManager = authenticationRegistrationManager;
    }

    public async Task<AuthenticationRegistrationResponse> GetAsync(long authenticationRegistration)
    {
        var @object = await _authenticationRegistrationManager.GetAsync(authenticationRegistration);

        var response = new AuthenticationRegistrationResponse
        {
            Snowflake = @object.Snowflake,
            AuthenticationIdentity = @object.AuthenticationIdentity,
            MailAddress = @object.MailAddress,
            ProcessState = @object.ProcessState
        };

        return response;
    }

    public IQueryable<AuthenticationRegistrationResponse> GetAll()
    {
        var objects = _authenticationRegistrationManager.GetQueryable();

        var response = objects.Select(@object =>
            new AuthenticationRegistrationResponse
            {
                Snowflake = @object.Snowflake,
                AuthenticationIdentity = @object.AuthenticationIdentity,
                MailAddress = @object.MailAddress,
                ProcessState = @object.ProcessState
            });

        return response;
    }

    public async Task<AuthenticationRegistrationResponse> InsertAsync(AuthenticationRegistrationRequest request)
    {
        var @object = new AuthenticationRegistration
        {
            AuthenticationIdentity = request.AuthenticationIdentity,
            MailAddress = request.MailAddress,
            ProcessState = AuthenticationRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            Secret = $"{Guid.NewGuid()}"
        };

        await _authenticationRegistrationManager.InsertAsync(@object);

        var response = new AuthenticationRegistrationResponse
        {
            Snowflake = @object.Snowflake,
            AuthenticationIdentity = @object.AuthenticationIdentity,
            MailAddress = @object.MailAddress,
            ProcessState = @object.ProcessState
        };

        return response;
    }

    public async Task UpdateAsync(long authenticationRegistration, AuthenticationRegistrationRequest request)
        => await _authenticationRegistrationManager.UpdateAsync(authenticationRegistration,
            @object =>
            {
                @object.AuthenticationIdentity = request.AuthenticationIdentity;
                @object.MailAddress = request.MailAddress;
            });

    public async Task DeleteAsync(long authenticationRegistration)
        => await _authenticationRegistrationManager.DeleteAsync(authenticationRegistration);
}