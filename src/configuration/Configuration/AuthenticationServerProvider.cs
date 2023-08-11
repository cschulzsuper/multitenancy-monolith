using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public sealed class AuthenticationServerProvider : IAuthenticationServerProvider
{
    private const string DefaultService = "server";

    private readonly IConfiguration _configuration;

    public AuthenticationServerProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AuthenticationServer Get()
        => _configuration.GetSection("AuthenticationServer").Get<AuthenticationServer>() ?? new AuthenticationServer { Service = DefaultService };
}