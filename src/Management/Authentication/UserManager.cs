using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public class UserManager : IUserManager
{
    public static readonly User[] _users =
    {
        new User("admin", "default"),
        new User("guest", "default"),
    };

    public User Get(string username)
        => _users.Single(x => x.Username == username);
}