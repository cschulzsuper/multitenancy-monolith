using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public interface IConfigurationProxyProvider
{
    AccessServer GetAccessServer();
    AdmissionPortal GetAdmissionPortal();
    AdmissionServer GetAdmissionServer();
    AllowedClient[] GetAllowedClients();
    DistributedCache GetDistributedCache();
    BuildInfo GetBuildInfo();
    string GetMaintenanceSecret();
    ServiceMapping[] GetServiceMappings();
    SwaggerDoc[] GetSwaggerDocs();

    T[] GetSeedData<T>(string uri);
}