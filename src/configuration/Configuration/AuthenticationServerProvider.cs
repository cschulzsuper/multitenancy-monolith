using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public sealed class AuthenticationServerProvider : IAuthenticationServerProvider
{
    private const string DefaultHost = "https://localhost:7207";

    private readonly IConfiguration _configuration;

    public AuthenticationServerProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AuthenticationServer Get()
        => _configuration.GetSection("AuthenticationServer").Get<AuthenticationServer>() ?? new AuthenticationServer { Host = DefaultHost };
}