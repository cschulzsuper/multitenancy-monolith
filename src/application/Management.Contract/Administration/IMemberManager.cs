using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMemberManager
{
    IEnumerable<Member> GetAll(string group);
}