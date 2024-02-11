using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal sealed class AccountMemberManager : IAccountMemberManager
{
    private readonly IRepository<AccountMember> _repository;

    public AccountMemberManager(IRepository<AccountMember> repository)
    {
        _repository = repository;
    }

    public async Task<AccountMember> GetAsync(long accountMember)
    {
        AccountMemberValidation.EnsureAccountMember(accountMember);

        var @object = await _repository.GetAsync(accountMember);

        return @object;
    }

    public async Task<AccountMember> GetAsync(string accountMember)
    {
        AccountMemberValidation.EnsureAccountMember(accountMember);

        var @object = await _repository.GetAsync(x => x.UniqueName == accountMember);

        return @object;
    }

    public async Task<AccountMember?> GetOrDefaultAsync(string accountMember)
    {
        AccountMemberValidation.EnsureAccountMember(accountMember);

        var @object = await _repository.GetOrDefaultAsync(x => x.UniqueName == accountMember);

        return @object;
    }

    public IQueryable<AccountMember> GetQueryable()
        => _repository.GetQueryable();

    public async Task InsertAsync(AccountMember @object)
    {
        AccountMemberValidation.EnsureInsertable(@object);

        await _repository.InsertAsync(@object);
    }

    public async Task UpdateAsync(long accountMember, Action<AccountMember> action)
    {
        AccountMemberValidation.EnsureAccountMember(accountMember);

        var validatedAction = (AccountMember @object) =>
        {
            action.Invoke(@object);

            AccountMemberValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(accountMember, validatedAction);
    }

    public async Task UpdateAsync(string accountMember, Action<AccountMember> action)
    {
        AccountMemberValidation.EnsureAccountMember(accountMember);

        var validatedAction = (AccountMember @object) =>
        {
            action.Invoke(@object);

            AccountMemberValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == accountMember, validatedAction);
    }

    public async Task DeleteAsync(long accountMember)
    {
        AccountMemberValidation.EnsureAccountMember(accountMember);

        await _repository.DeleteOrThrowAsync(accountMember);
    }

    public async Task DeleteAsync(string accountMember)
    {
        AccountMemberValidation.EnsureAccountMember(accountMember);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == accountMember);
    }
}