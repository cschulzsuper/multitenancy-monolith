using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public interface IAllowedClientsProvider
{
    ICollection<AllowedClient> Get();
}
