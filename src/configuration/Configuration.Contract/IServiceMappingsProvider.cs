using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public interface IServiceMappingsProvider
{
    ServiceMapping[] Get();
    string[] GetUniqueNames();
}