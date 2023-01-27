using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

public interface IMembershipManager
{
    ValueTask<Membership> GetAsync(long snowflake);

    IQueryable<Membership> GetQueryable();

    ValueTask InsertAsync(Membership membership);

    ValueTask DeleteAsync(long snowflake);
}