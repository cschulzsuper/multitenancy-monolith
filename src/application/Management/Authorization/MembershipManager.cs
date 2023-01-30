using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

internal sealed class MembershipManager : IMembershipManager
{
    private readonly IRepository<Membership> _repository;

    public MembershipManager(IRepository<Membership> repository)
    {
        _repository = repository;
    }

    public async ValueTask<Membership> GetAsync(long snowflake)
    {
        MembershipValidation.EnsureSnowflake(snowflake);

        var membership = await _repository.GetAsync(snowflake);

        return membership;
    }

    public IQueryable<Membership> GetQueryable()
        => _repository.GetQueryable();

    public async ValueTask InsertAsync(Membership membership)
    {
        MembershipValidation.EnsureInsertable(membership);

        await _repository.InsertAsync(membership);
    }

    public async ValueTask DeleteAsync(long snowflake)
    {
        MembershipValidation.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }
}