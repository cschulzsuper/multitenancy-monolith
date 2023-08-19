using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public sealed class ConfigurationProxyProvider : IConfigurationProxyProvider
{
    private const string AdmissionServer = nameof(AdmissionServer);
    private const string AllowedClients = nameof(AllowedClients);
    private const string ServiceMappings = nameof(ServiceMappings);
    private const string SwaggerDocs = nameof(SwaggerDocs);

    private readonly IConfiguration _configuration;

    public ConfigurationProxyProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AdmissionServer GetAdmissionServer()
    {
        var admissionServer = _configuration.GetSection(AdmissionServer).Get<AdmissionServer>();

        if (admissionServer == null)
        {
            ConfigurationException.ThrowNotConfigured(AdmissionServer);
        }

        return admissionServer;
    }

    public AllowedClient[] GetAllowedClients()
    {
        var allowedClients = _configuration.GetSection(AllowedClients).Get<AllowedClient[]>();

        if (allowedClients == null)
        {
            ConfigurationException.ThrowNotConfigured(AllowedClients);
        }

        return allowedClients;
    }

    public ServiceMapping[] GetServiceMappings()
    {
        var serviceMapping = _configuration.GetSection(ServiceMappings).Get<ServiceMapping[]>();

        if (serviceMapping == null)
        {
            ConfigurationException.ThrowNotConfigured(ServiceMappings);
        }

        return serviceMapping;
    }

    public SwaggerDoc[] GetSwaggerDocs()
    {
        var swaggerDocs = _configuration.GetSection(SwaggerDocs).Get<SwaggerDoc[]>();

        if (swaggerDocs == null)
        {
            ConfigurationException.ThrowNotConfigured(SwaggerDocs);
        }

        return swaggerDocs;
    }
}