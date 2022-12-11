using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using ChristianSchulz.MultitenancyMonolith.Data;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MembershipManager : IMembershipManager
{
    private readonly IRepository<Membership> _repository;

    public MembershipManager(IRepository<Membership> repository)
    {
        _repository = repository;
    }

    public Membership Get(long snowflake)
        => _repository.Get(snowflake);

    public IQueryable<Membership> GetAll()
        => _repository.GetQueryable();
}