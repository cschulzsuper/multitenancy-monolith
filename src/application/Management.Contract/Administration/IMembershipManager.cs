using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMembershipManager
{
    IQueryable<Membership> GetAll();
}