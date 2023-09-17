using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public interface IConfigurationProxyProvider
{
    AccessServer GetAccessServer();
    AdmissionPortal GetAdmissionPortal();
    AdmissionServer GetAdmissionServer();
    DistributedCache GetDistributedCache();
    AllowedClient[] GetAllowedClients();
    string GetMaintenanceSecret();
    ServiceMapping[] GetServiceMappings();
    SwaggerDoc[] GetSwaggerDocs();

    T[] GetSeedData<T>(string uri);
}