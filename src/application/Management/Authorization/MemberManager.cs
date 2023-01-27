using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using ChristianSchulz.MultitenancyMonolith.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

internal sealed class MemberManager : IMemberManager
{
    private readonly IRepository<Member> _repository;

    public MemberManager(IRepository<Member> repository)
    {
        _repository = repository;
    }

    public async ValueTask<Member> GetAsync(long snowflake)
    {
        MemberValidator.EnsureSnowflake(snowflake);

        var member = await _repository.GetAsync(snowflake);

        return member;
    }

    public async ValueTask<Member> GetAsync(string uniqueName)
    {
        MemberValidator.EnsureUniqueName(uniqueName);

        var member = await _repository.GetAsync(x => x.UniqueName == uniqueName);

        return member;
    }

    public IAsyncEnumerable<Member> GetAsyncEnumerable()
        => _repository.GetAsyncEnumerable();

    public async ValueTask InsertAsync(Member member)
    {
        MemberValidator.EnsureInsertable(member);

        await _repository.InsertAsync(member);
    }

    public async ValueTask UpdateAsync(long snowflake, Action<Member> action)
    {
        var validatedAction = (Member member) =>
        {
            action.Invoke(member);

            MemberValidator.EnsureUpdatable(member);
        };

        await _repository.UpdateOrThrowAsync(snowflake, validatedAction);
    }

    public async ValueTask UpdateAsync(string uniqueName, Action<Member> action)
    {
        var validatedAction = (Member member) =>
        {
            action.Invoke(member);

            MemberValidator.EnsureUpdatable(member);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == uniqueName, validatedAction);
    }

    public async ValueTask DeleteAsync(long snowflake)
    {
        MemberValidator.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }

    public async ValueTask DeleteAsync(string uniqueName)
    {
        MemberValidator.EnsureUniqueName(uniqueName);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == uniqueName);
    }
}