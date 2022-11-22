using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMemberManager
{
    Member Get(string group, string uniqueName);

    IEnumerable<Member> GetAll(string group);
}