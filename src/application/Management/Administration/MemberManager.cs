using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using ChristianSchulz.MultitenancyMonolith.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

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

    public IQueryable<Member> GetQueryable()
        => _repository.GetQueryable();

    public async ValueTask InsertAsync(Member member)
    {
        MemberValidator.EnsureInsertable(member);

        await _repository.InsertAsync(member);
    }

    public async ValueTask DeleteAsync(long snowflake)
    {
        MemberValidator.EnsureSnowflake(snowflake);

        await _repository.DeleteAsync(snowflake);
    }

    public async ValueTask DeleteAsync(string uniqueName)
    {
        MemberValidator.EnsureUniqueName(uniqueName);

        await _repository.DeleteAsync(x => x.UniqueName == uniqueName);
    }
}