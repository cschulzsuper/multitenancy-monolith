using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using System;
using System.Collections.Generic;
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
        MemberValidation.EnsureSnowflake(snowflake);

        var member = await _repository.GetAsync(snowflake);

        return member;
    }

    public async ValueTask<Member> GetAsync(string member)
    {
        MemberValidation.EnsureMember(member);

        var @object = await _repository.GetAsync(x => x.UniqueName == member);

        return @object;
    }

    public async ValueTask<Member?> GetOrDefaultAsync(string member)
    {
        MemberValidation.EnsureMember(member);

        var @object = await _repository.GetOrDefaultAsync(x => x.UniqueName == member);

        return @object;
    }

    public IAsyncEnumerable<Member> GetAsyncEnumerable()
        => _repository.GetAsyncEnumerable();

    public async ValueTask InsertAsync(Member member)
    {
        MemberValidation.EnsureInsertable(member);

        await _repository.InsertAsync(member);
    }

    public async ValueTask UpdateAsync(long snowflake, Action<Member> action)
    {
        MemberValidation.EnsureSnowflake(snowflake);

        var validatedAction = (Member member) =>
        {
            action.Invoke(member);

            MemberValidation.EnsureUpdatable(member);
        };

        await _repository.UpdateOrThrowAsync(snowflake, validatedAction);
    }

    public async ValueTask UpdateAsync(string member, Action<Member> action)
    {
        MemberValidation.EnsureMember(member);

        var validatedAction = (Member @object) =>
        {
            action.Invoke(@object);

            MemberValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(x => x.UniqueName == member, validatedAction);
    }

    public async ValueTask DeleteAsync(long snowflake)
    {
        MemberValidation.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }

    public async ValueTask DeleteAsync(string member)
    {
        MemberValidation.EnsureMember(member);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == member);
    }
}