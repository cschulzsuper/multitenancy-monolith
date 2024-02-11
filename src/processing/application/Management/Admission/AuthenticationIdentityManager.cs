using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using System;
using System.Collections.Generic;
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

    public async Task<bool> ExistsAsync(string authenticationIdentity)
    {
        AuthenticationIdentityValidation.EnsureAuthenticationIdentity(authenticationIdentity);

        var exists = await _repository
            .ExistsAsync(@object => @object.UniqueName == authenticationIdentity);

        return exists;
    }

    public async Task<bool> ExistsAsync(string authenticationIdentity, string secret)
    {
        AuthenticationIdentityValidation.EnsureAuthenticationIdentity(authenticationIdentity);

        var exists = await _repository
            .ExistsAsync(@object =>
                @object.UniqueName == authenticationIdentity &&
                @object.Secret == secret);

        return exists;
    }

    public async Task<AuthenticationIdentity> GetAsync(long authenticationIdentity)
    {
        AuthenticationIdentityValidation.EnsureAuthenticationIdentity(authenticationIdentity);

        var @object = await _repository.GetAsync(authenticationIdentity);

        return @object;
    }

    public async Task<AuthenticationIdentity> GetAsync(string authenticationIdentity)
    {
        AuthenticationIdentityValidation.EnsureAuthenticationIdentity(authenticationIdentity);

        var @object = await _repository.GetAsync(x => x.UniqueName == authenticationIdentity);

        return @object;
    }

    public IAsyncEnumerable<TResult> GetAsyncEnumerable<TResult>(Func<IQueryable<AuthenticationIdentity>, IQueryable<TResult>> query)
        => _repository.GetAsyncEnumerable(query);

    public async Task InsertAsync(AuthenticationIdentity @object)
    {
        AuthenticationIdentityValidation.EnsureInsertable(@object);

        await _repository.InsertAsync(@object);
    }

    public async Task UpdateAsync(long authenticationIdentity, Action<AuthenticationIdentity> action)
    {
        AuthenticationIdentityValidation.EnsureAuthenticationIdentity(authenticationIdentity);

        var validatedAction = (AuthenticationIdentity @object) =>
        {
            action.Invoke(@object);

            AuthenticationIdentityValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(authenticationIdentity, validatedAction);
    }

    public async Task UpdateAsync(string authenticationIdentity, Action<AuthenticationIdentity> action)
    {
        AuthenticationIdentityValidation.EnsureAuthenticationIdentity(authenticationIdentity);

        var validatedAction = (AuthenticationIdentity @object) =>
        {
            action.Invoke(@object);

            AuthenticationIdentityValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == authenticationIdentity, validatedAction);
    }

    public async Task DeleteAsync(long authenticationIdentity)
    {
        AuthenticationIdentityValidation.EnsureAuthenticationIdentity(authenticationIdentity);

        await _repository.DeleteOrThrowAsync(authenticationIdentity);
    }

    public async Task DeleteAsync(string authenticationIdentity)
    {
        AuthenticationIdentityValidation.EnsureAuthenticationIdentity(authenticationIdentity);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == authenticationIdentity);
    }
}