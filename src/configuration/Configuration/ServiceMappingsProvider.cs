using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public sealed class ServiceMappingsProvider : IServiceMappingsProvider
{
    private readonly IConfiguration _configuration;

    private ServiceMapping[]? _serviceMappings;

    private string[]? _serviceMappingUniqueNames;

    public ServiceMappingsProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ServiceMapping[] Get()
        => _serviceMappings ??= _configuration.GetSection("ServiceMappings").Get<ServiceMapping[]>() ?? Array.Empty<ServiceMapping>();

    public string[] GetUniqueNames()
        => _serviceMappingUniqueNames ??= Get().Select(x => x.UniqueName).ToArray();
}