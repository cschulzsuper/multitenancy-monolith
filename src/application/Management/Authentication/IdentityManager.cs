using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public class IdentityManager : IIdentityManager
{
    public static readonly Identity[] _users =
    {
        new Identity { UniqueName = "admin", Secret = "default" },
        new Identity { UniqueName = "guest", Secret = "default" },
    };

    public Identity Get(string uniqueName)
        => _users.Single(x => x.UniqueName == uniqueName);
}