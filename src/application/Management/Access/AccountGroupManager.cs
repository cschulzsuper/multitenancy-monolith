using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal sealed class AccountGroupManager : IAccountGroupManager
{
    private readonly IRepository<AccountGroup> _repository;

    public AccountGroupManager(IRepository<AccountGroup> repository)
    {
        _repository = repository;
    }

    public async Task<AccountGroup> GetAsync(long accountGroup)
    {
        AccountGroupValidation.EnsureSnowflake(accountGroup);

        var @object = await _repository.GetAsync(accountGroup);

        return @object;
    }

    public async Task<AccountGroup> GetAsync(string accountGroup)
    {
        AccountGroupValidation.EnsureIdentity(accountGroup);

        var @object = await _repository.GetAsync(x => x.UniqueName == accountGroup);

        return @object;
    }

    public IQueryable<AccountGroup> GetAll()
        => _repository.GetQueryable();

    public async Task InsertAsync(AccountGroup @object)
    {
        AccountGroupValidation.EnsureInsertable(@object);

        await _repository.InsertAsync(@object);
    }

    public async Task UpdateAsync(long accountGroup, Action<AccountGroup> action)
    {
        AccountGroupValidation.EnsureSnowflake(accountGroup);

        var validatedAction = (AccountGroup @object) =>
        {
            action.Invoke(@object);

            AccountGroupValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(accountGroup, validatedAction);
    }

    public async Task UpdateAsync(string accountGroup, Action<AccountGroup> action)
    {
        AccountGroupValidation.EnsureIdentity(accountGroup);

        var validatedAction = (AccountGroup @object) =>
        {
            action.Invoke(@object);

            AccountGroupValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == accountGroup, validatedAction);
    }

    public async Task DeleteAsync(long accountGroup)
    {
        AccountGroupValidation.EnsureSnowflake(accountGroup);

        await _repository.DeleteOrThrowAsync(accountGroup);
    }

    public async Task DeleteAsync(string accountGroup)
    {
        AccountGroupValidation.EnsureIdentity(accountGroup);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == accountGroup);
    }
}