using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Request;
using System;
using System.Linq;
using System.Text;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public class AuthenticationRequestHandler : IAuthenticationRequestHandler
{
    private readonly IUserManager _userManager;

    private readonly static object _signInLock = new();

    public AuthenticationRequestHandler(
        IUserManager userManager)
    {
        _userManager = userManager;
    }

    public string SignIn(string username, SignInRequest request)
    {
        lock (_signInLock)
        {
            var user = _userManager.Get(username);

            var valid = request.Password == user.Password;

            if (!valid)
            {
                throw new TransportException($"The password does not match.");
            }

            user.Verification = Guid
                .NewGuid()
                .ToByteArray();

            var badge = CreateBadge(user);

            return badge;
        }
    }

    private static string CreateBadge(User user)
    {
        var usernameBytes = Encoding.UTF8.GetBytes(user.Username);

        var badgeBytes = Enumerable
            .Concat(user.Verification, usernameBytes)
            .ToArray();

        return Convert.ToBase64String(badgeBytes);
    }
}