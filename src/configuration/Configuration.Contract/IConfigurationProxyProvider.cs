using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

namespace ChristianSchulz.MultitenancyMonolith.Configuration
{
    public interface IConfigurationProxyProvider
    {
        AdmissionServer GetAdmissionServer();
        AllowedClient[] GetAllowedClients();
        ServiceMapping[] GetServiceMappings();
        SwaggerDoc[] GetSwaggerDocs();
    }
}