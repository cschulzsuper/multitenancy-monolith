using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMembershipManager
{
    Membership Get(long snowflake);
    IQueryable<Membership> GetAll();
}