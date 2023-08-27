using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public interface IConfigurationProxyProvider
{
    AccessServer GetAccessServer();
    AdmissionPortal GetAdmissionPortal();
    AdmissionServer GetAdmissionServer();
    AllowedClient[] GetAllowedClients();
    DefaultStagingAuthenticationIdentity GetDefaultStagingAuthenticationIdentity();
    MaintenanceAuthenticationIdentity GetMaintenanceAuthenticationIdentity();
    ServiceMapping[] GetServiceMappings();
    SwaggerDoc[] GetSwaggerDocs();

    T[] GetSeedData<T>(string uri);
}