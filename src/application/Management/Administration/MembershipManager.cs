using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using ChristianSchulz.MultitenancyMonolith.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MembershipManager : IMembershipManager
{
    private readonly IRepository<Membership> _repository;

    public MembershipManager(IRepository<Membership> repository)
    {
        _repository = repository;
    }

    public async ValueTask<Membership> GetAsync(long snowflake)
    {
        MembershipValidator.EnsureSnowflake(snowflake);

        var membership = await _repository.GetAsync(snowflake);

        return membership;
    }

    public IQueryable<Membership> GetQueryable()
        => _repository.GetQueryable();

    public async ValueTask InsertAsync(Membership membership)
    {
        MembershipValidator.EnsureInsertable(membership);

        await _repository.InsertAsync(membership);
    }

    public async ValueTask DeleteAsync(long snowflake)
    {
        MembershipValidator.EnsureSnowflake(snowflake);

        await _repository.DeleteAsync(snowflake);
    }
}