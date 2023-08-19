using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public sealed class AdmissionServerProvider : IAdmissionServerProvider
{
    private const string DefaultService = "server";

    private readonly IConfiguration _configuration;

    public AdmissionServerProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AdmissionServer Get()
        => _configuration.GetSection("AdmissionServer").Get<AdmissionServer>() ?? new AdmissionServer { Service = DefaultService };
}