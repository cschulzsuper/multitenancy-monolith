using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public interface IWebServicesProvider
{
    WebService[] Get();
    string[] GetUniqueNames();
}