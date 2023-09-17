using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public sealed class ConfigurationProxyProvider : IConfigurationProxyProvider
{
    private const string AccessServer = nameof(AccessServer);
    private const string AdmissionPortal = nameof(AdmissionPortal);
    private const string AdmissionServer = nameof(AdmissionServer);
    private const string AllowedClients = nameof(AllowedClients);
    private const string DistributedCache = nameof(DistributedCache);
    private const string MaintenanceSecret = nameof(MaintenanceSecret);
    private const string ServiceMappings = nameof(ServiceMappings);
    private const string SwaggerDocs = nameof(SwaggerDocs);

    private const string SeedData = nameof(SeedData);
    private const string SeedDataScheme = "Scheme";
    private const string SeedDataResource =  "Resource";

    private readonly IConfiguration _configuration;

    public ConfigurationProxyProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AccessServer GetAccessServer()
    {
        var accessServer = _configuration
            .GetSection(AccessServer)
            .Get<AccessServer>();

        if (accessServer == null)
        {
            ConfigurationException.ThrowNotConfigured(AccessServer);
        }

        return accessServer;
    }

    public AdmissionPortal GetAdmissionPortal()
    {
        var admissionPortal = _configuration
            .GetSection(AdmissionPortal)
            .Get<AdmissionPortal>();

        if (admissionPortal == null)
        {
            ConfigurationException.ThrowNotConfigured(AdmissionPortal);
        }

        return admissionPortal;
    }

    public AdmissionServer GetAdmissionServer()
    {
        var admissionServer = _configuration
            .GetSection(AdmissionServer)
            .Get<AdmissionServer>();

        if (admissionServer == null)
        {
            ConfigurationException.ThrowNotConfigured(AdmissionServer);
        }

        return admissionServer;
    }

    public DistributedCache GetDistributedCache()
    {
        var distributedCache = _configuration
            .GetSection(DistributedCache)
            .Get<DistributedCache>();

        if (distributedCache == null)
        {
            ConfigurationException.ThrowNotConfigured(DistributedCache);
        }

        return distributedCache;
    }

    public AllowedClient[] GetAllowedClients()
    {
        var allowedClients = _configuration
            .GetSection(AllowedClients)
            .Get<AllowedClient[]>();

        return allowedClients ?? [];
    }

    public string GetMaintenanceSecret()
    {
        var maintenanceSecret = _configuration
            .GetSection(MaintenanceSecret)
            .Get<string>();

        if (maintenanceSecret == null)
        {
            ConfigurationException.ThrowNotConfigured(MaintenanceSecret);
        }

        return maintenanceSecret;
    }

    public ServiceMapping[] GetServiceMappings()
    {
        var serviceMappings = _configuration
            .GetSection(ServiceMappings)
            .Get<ServiceMapping[]>();

        return serviceMappings ?? [];
    }

    public SwaggerDoc[] GetSwaggerDocs()
    {
        var swaggerDocs = _configuration
            .GetSection(SwaggerDocs)
            .Get<SwaggerDoc[]>();

        return swaggerDocs ?? [];
    }

    public T[] GetSeedData<T>(string uri)
    {
        var seeds = _configuration
            .GetSection(SeedData)
            .GetChildren()
            .Where(seedConfiguration => seedConfiguration.GetSection(SeedDataScheme).Value == uri)
            .Select(seedConfiguration => seedConfiguration.GetSection(SeedDataResource).Get<T>()!)
            .Where(seed => seed != null)
            .ToArray();

        return seeds ?? [];
    }
}