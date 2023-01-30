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

    public async ValueTask<Identity> GetAsync(string uniqueName)
    {
        IdentityValidation.EnsureUniqueName(uniqueName);

        var identity = await _repository.GetAsync(x => x.UniqueName == uniqueName);

        return identity;
    }

    public IQueryable<Identity> GetAll()
        => _repository.GetQueryable();

    public async ValueTask InsertAsync(Identity identity)
    {
        IdentityValidation.EnsureInsertable(identity);

        await _repository.InsertAsync(identity);
    }

    public async ValueTask UpdateAsync(long snowflake, Action<Identity> action)
    {
        var validatedAction = (Identity identity) =>
        {
            action.Invoke(identity);

            IdentityValidation.EnsureUpdatable(identity);
        };

        await _repository.UpdateOrThrowAsync(snowflake, validatedAction);
    }

    public async ValueTask UpdateAsync(string uniqueName, Action<Identity> action)
    {
        var validatedAction = (Identity identity) =>
        {
            action.Invoke(identity);

            IdentityValidation.EnsureUpdatable(identity);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == uniqueName, validatedAction);
    }

    public async ValueTask DeleteAsync(long snowflake)
    {
        IdentityValidation.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }

    public async ValueTask DeleteAsync(string uniqueName)
    {
        IdentityValidation.EnsureUniqueName(uniqueName);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == uniqueName);
    }
}