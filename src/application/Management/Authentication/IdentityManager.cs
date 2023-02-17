using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentityManager : IIdentityManager
{
    private readonly IRepository<Identity> _repository;

    public IdentityManager(IRepository<Identity> repository)
    {
        _repository = repository;
    }

    public async ValueTask<Identity> GetAsync(long snowflake)
    {
        IdentityValidation.EnsureSnowflake(snowflake);

        var identity = await _repository.GetAsync(snowflake);

        return identity;
    }

    public async ValueTask<Identity> GetAsync(string identity)
    {
        IdentityValidation.EnsureIdentity(identity);

        var @object = await _repository.GetAsync(x => x.UniqueName == identity);

        return @object;
    }

    public IQueryable<Identity> GetAll()
        => _repository.GetQueryable();

    public async ValueTask<bool> ExistsAsync(string identity, string secret)
    {
        IdentityValidation.EnsureIdentity(identity);

        var exists = await _repository
            .ExistsAsync(x =>
                x.UniqueName == identity &&
                x.Secret == secret);

        return exists;
    }

    public async ValueTask InsertAsync(Identity @object)
    {
        IdentityValidation.EnsureInsertable(@object);

        await _repository.InsertAsync(@object);
    }

    public async ValueTask UpdateAsync(long snowflake, Action<Identity> action)
    {
        IdentityValidation.EnsureSnowflake(snowflake);

        var validatedAction = (Identity @object) =>
        {
            action.Invoke(@object);

            IdentityValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(snowflake, validatedAction);
    }

    public async ValueTask UpdateAsync(string identity, Action<Identity> action)
    {
        IdentityValidation.EnsureIdentity(identity);

        var validatedAction = (Identity @object) =>
        {
            action.Invoke(@object);

            IdentityValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == identity, validatedAction);
    }

    public async ValueTask DeleteAsync(long snowflake)
    {
        IdentityValidation.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }

    public async ValueTask DeleteAsync(string identity)
    {
        IdentityValidation.EnsureIdentity(identity);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == identity);
    }
}