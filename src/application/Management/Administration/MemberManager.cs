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
        => _repository.Get(snowflake);

    public Member Get(string uniqueName)
        => _repository.GetQueryable().Single(x => x.UniqueName == uniqueName);

    public IQueryable<Member> GetAll()
        => _repository.GetQueryable();

    public void Insert(Member member)
    {
        MemberValidator.Ensure(member);

        _repository.Insert(member);
    }
}