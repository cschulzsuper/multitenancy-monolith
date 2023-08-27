using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public sealed class ConfigurationProxyProvider : IConfigurationProxyProvider
{
    private const string AccessServer = nameof(AccessServer);
    private const string AdmissionServer = nameof(AdmissionServer);
    private const string AllowedClients = nameof(AllowedClients);
    private const string ServiceMappings = nameof(ServiceMappings);
    private const string SwaggerDocs = nameof(SwaggerDocs);

    private const string SeedData = nameof(SeedData);
    private const string SeedDataUri = "Uri";
    private const string SeedDataResource =  "Resource";

    private readonly IConfiguration _configuration;

    public ConfigurationProxyProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AccessServer GetAccessServer()
    {
        var accessServer = _configuration.GetSection(AccessServer).Get<AccessServer>();

        if (accessServer == null)
        {
            ConfigurationException.ThrowNotConfigured(AccessServer);
        }

        return accessServer;
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

        return allowedClients ?? Array.Empty<AllowedClient>();
    }

    public ServiceMapping[] GetServiceMappings()
    {
        var serviceMappings = _configuration.GetSection(ServiceMappings).Get<ServiceMapping[]>();

        return serviceMappings ?? Array.Empty<ServiceMapping>();
    }

    public SwaggerDoc[] GetSwaggerDocs()
    {
        var swaggerDocs = _configuration.GetSection(SwaggerDocs).Get<SwaggerDoc[]>();

        return swaggerDocs ?? Array.Empty<SwaggerDoc>();
    }

    public T[] GetSeedData<T>(string uri)
    {
        var seedData = _configuration
            .GetSection(SeedData)
            .GetChildren()
            .Where(x => x.GetSection(SeedDataUri).Value == uri)
            .Select(x => x.GetSection(SeedDataResource).Get<T>()!)
            .Where(x => x != null)
            .ToArray();

        return seedData ?? Array.Empty<T>();
    }
}