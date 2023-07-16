using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class AuthenticationRegistrationManager : IAuthenticationRegistrationManager
{
    private readonly IRepository<AuthenticationRegistration> _repository;

    public AuthenticationRegistrationManager(IRepository<AuthenticationRegistration> repository)
    {
        _repository = repository;
    }

    public async Task<AuthenticationRegistration> GetAsync(long authenticationRegistration)
    {
        AuthenticationRegistrationValidation.EnsureAuthenticationRegistration(authenticationRegistration);

        var @object = await _repository.GetAsync(authenticationRegistration);

        return @object;
    }

    public IQueryable<AuthenticationRegistration> GetQueryable()
        => _repository.GetQueryable();

    public async Task InsertAsync(AuthenticationRegistration @object)
    {
        AuthenticationRegistrationValidation.EnsureInsertable(@object);

        await _repository.InsertAsync(@object);
    }


    public async Task UpdateAsync(long authenticationRegistration, Action<AuthenticationRegistration> action)
    {
        AuthenticationRegistrationValidation.EnsureAuthenticationRegistration(authenticationRegistration);

        var validatedAction = (AuthenticationRegistration @object) =>
        {
            action.Invoke(@object);

            AuthenticationRegistrationValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(authenticationRegistration, validatedAction);
    }

    public async Task UpdateAsync(string authenticationIdentity, Action<AuthenticationRegistration> action)
    {
        AuthenticationRegistrationValidation.EnsureAuthenticationIdentity(authenticationIdentity);

        var validatedAction = (AuthenticationRegistration @object) =>
        {
            action.Invoke(@object);

            AuthenticationRegistrationValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(@object => @object.AuthenticationIdentity == authenticationIdentity, validatedAction);
    }

    public async Task DeleteAsync(long authenticationRegistration)
    {
        AuthenticationRegistrationValidation.EnsureAuthenticationRegistration(authenticationRegistration);

        await _repository.DeleteOrThrowAsync(authenticationRegistration);
    }
}
