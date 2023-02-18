using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public sealed class AllowedClientsProvider : IAllowedClientsProvider
{
    private readonly IConfiguration _configuration;

    public AllowedClientsProvider(IConfiguration configuration) 
    {
        _configuration = configuration;
    }

    public ICollection<AllowedClient> Get()
        => _configuration.GetSection("AllowedClients").Get<AllowedClient[]>() ?? Array.Empty<AllowedClient>();
}
