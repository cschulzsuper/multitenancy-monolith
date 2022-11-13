using System;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Request;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public class AuthenticationRequestHandler : IAuthenticationRequestHandler
{
    private const string _defaultUsername = "admin";
    private const string _defaultPassword = "default";

    private static readonly byte[] _defaultBadgeBytes = Guid
        .Parse("7c348e46-6706-42f1-8cb7-14092ee319b3")
        .ToByteArray();

    public string SignIn(string username, SignInRequest request)
    {
        var password = request.Password;

        var valid =
            username == _defaultUsername &&
            password == _defaultPassword;

        if (!valid)
        {
            throw new TransportException($"The password does not match.");
        }

        return Convert.ToBase64String(_defaultBadgeBytes);
    }
}