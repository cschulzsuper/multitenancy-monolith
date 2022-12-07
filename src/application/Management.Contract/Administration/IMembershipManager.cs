using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMembershipManager
{
    IEnumerable<Membership> GetAll(string group);
}