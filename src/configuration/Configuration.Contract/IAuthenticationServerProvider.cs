using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public interface IAuthenticationServerProvider
{
    AuthenticationServer Get();
}