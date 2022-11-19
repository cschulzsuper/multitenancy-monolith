using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public class MemberManager : IMemberManager
{
    public const string DefaultGroup = "default";

    public static readonly Member[] _members =
    {
        new Member { UniqueName = "default-admin", Identity = "admin" },
        new Member { UniqueName = "default-guest", Identity = "guest" },
    };

    public IEnumerable<Member> GetAll(string group)
    {
        if (group != DefaultGroup)
        {
            throw new ManagementException("Group does not exist");
        }

        return _members.AsReadOnly();
    }
}