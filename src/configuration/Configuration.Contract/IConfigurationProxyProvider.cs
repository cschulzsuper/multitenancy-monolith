using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public interface IConfigurationProxyProvider
{
    AccessServer GetAccessServer();
    AdmissionPortal GetAdmissionPortal();
    AdmissionServer GetAdmissionServer();
    AllowedClient[] GetAllowedClients();
    MaintenanceAuthenticationIdentity GetMaintenanceAuthenticationIdentity();
    ServiceMapping[] GetServiceMappings();
    SwaggerDoc[] GetSwaggerDocs();

    T[] GetSeedData<T>(string uri);
}