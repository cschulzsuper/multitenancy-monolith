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

    public Task<bool> ExistsAsync(string accountGroup)
    {
        AccountGroupValidation.EnsureAccountGroup(accountGroup);

        var exists = _repository.ExistsAsync(@object => @object.UniqueName == accountGroup);

        return exists;
    }

    public async Task<AccountGroup> GetAsync(long accountGroup)
    {
        AccountGroupValidation.EnsureAccountGroup(accountGroup);

        var @object = await _repository.GetAsync(accountGroup);

        return @object;
    }

    public async Task<AccountGroup> GetAsync(string accountGroup)
    {
        AccountGroupValidation.EnsureAccountGroup(accountGroup);

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
        AccountGroupValidation.EnsureAccountGroup(accountGroup);

        var validatedAction = (AccountGroup @object) =>
        {
            action.Invoke(@object);

            AccountGroupValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(accountGroup, validatedAction);
    }

    public async Task UpdateAsync(string accountGroup, Action<AccountGroup> action)
    {
        AccountGroupValidation.EnsureAccountGroup(accountGroup);

        var validatedAction = (AccountGroup @object) =>
        {
            action.Invoke(@object);

            AccountGroupValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(@object => @object.UniqueName == accountGroup, validatedAction);
    }

    public async Task DeleteAsync(long accountGroup)
    {
        AccountGroupValidation.EnsureAccountGroup(accountGroup);

        await _repository.DeleteOrThrowAsync(accountGroup);
    }

    public async Task DeleteAsync(string accountGroup)
    {
        AccountGroupValidation.EnsureAccountGroup(accountGroup);

        await _repository.DeleteOrThrowAsync(@object => @object.UniqueName == accountGroup);
    }
}