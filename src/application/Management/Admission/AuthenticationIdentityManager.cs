using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class AuthenticationIdentityManager : IAuthenticationIdentityManager
{
    private readonly IRepository<AuthenticationIdentity> _repository;

    public AuthenticationIdentityManager(IRepository<AuthenticationIdentity> repository)
    {
        _repository = repository;
    }

    public async Task<AuthenticationIdentity> GetAsync(long authenticationIdentity)
    {
        AuthenticationIdentityValidation.EnsureSnowflake(authenticationIdentity);

        var @object = await _repository.GetAsync(authenticationIdentity);

        return @object;
    }

    public async Task<AuthenticationIdentity> GetAsync(string authenticationIdentity)
    {
        AuthenticationIdentityValidation.EnsureIdentity(authenticationIdentity);

        var @object = await _repository.GetAsync(x => x.UniqueName == authenticationIdentity);

        return @object;
    }

    public IQueryable<AuthenticationIdentity> GetQueryable()
        => _repository.GetQueryable();

    public async Task<bool> ExistsAsync(string authenticationIdentity, string secret)
    {
        AuthenticationIdentityValidation.EnsureIdentity(authenticationIdentity);

        var exists = await _repository
            .ExistsAsync(x =>
                x.UniqueName == authenticationIdentity &&
                x.Secret == secret);

        return exists;
    }

    public async Task InsertAsync(AuthenticationIdentity @object)
    {
        AuthenticationIdentityValidation.EnsureInsertable(@object);

        await _repository.InsertAsync(@object);
    }

    public async Task UpdateAsync(long authenticationIdentity, Action<AuthenticationIdentity> action)
    {
        AuthenticationIdentityValidation.EnsureSnowflake(authenticationIdentity);

        var validatedAction = (AuthenticationIdentity @object) =>
        {
            action.Invoke(@object);

            AuthenticationIdentityValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(authenticationIdentity, validatedAction);
    }

    public async Task UpdateAsync(string authenticationIdentity, Action<AuthenticationIdentity> action)
    {
        AuthenticationIdentityValidation.EnsureIdentity(authenticationIdentity);

        var validatedAction = (AuthenticationIdentity @object) =>
        {
            action.Invoke(@object);

            AuthenticationIdentityValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == authenticationIdentity, validatedAction);
    }

    public async Task DeleteAsync(long authenticationIdentity)
    {
        AuthenticationIdentityValidation.EnsureSnowflake(authenticationIdentity);

        await _repository.DeleteOrThrowAsync(authenticationIdentity);
    }

    public async Task DeleteAsync(string authenticationIdentity)
    {
        AuthenticationIdentityValidation.EnsureIdentity(authenticationIdentity);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == authenticationIdentity);
    }
}