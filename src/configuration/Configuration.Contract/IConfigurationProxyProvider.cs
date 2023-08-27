using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public interface IConfigurationProxyProvider
{
    AccessServer GetAccessServer();
    AdmissionServer GetAdmissionServer();
    AllowedClient[] GetAllowedClients();
    ServiceMapping[] GetServiceMappings();
    SwaggerDoc[] GetSwaggerDocs();

    T[] GetSeedData<T>(string uri);
}