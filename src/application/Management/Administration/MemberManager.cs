using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using ChristianSchulz.MultitenancyMonolith.Data;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MemberManager : IMemberManager
{
    private readonly IRepository<Member> _repository;

    public MemberManager(IRepository<Member> repository)
    {
        _repository = repository;
    }

    public Member Get(long snowflake)
    {
        MemberValidator.EnsureSnowflake(snowflake);

        var member = _repository.Get(snowflake);

        return member;
    }

    public Member Get(string uniqueName)
    {
        MemberValidator.EnsureUniqueName(uniqueName);

        var memeber = _repository
            .GetQueryable()
            .Single(x => x.UniqueName == uniqueName);

        return memeber;
    }

    public IQueryable<Member> GetAll()
        => _repository.GetQueryable();

    public void Insert(Member member)
    {
        MemberValidator.Ensure(member);

        _repository.Insert(member);
    }
}