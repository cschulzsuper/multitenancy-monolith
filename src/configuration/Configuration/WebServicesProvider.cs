using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public sealed class WebServicesProvider : IWebServicesProvider
{
    private readonly IConfiguration _configuration;

    private WebService[]? _webServices;

    private string[]? _webServiceUniqueNames;

    public WebServicesProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public WebService[] Get()
        => _webServices ??= _configuration.GetSection("WebServices").Get<WebService[]>() ?? Array.Empty<WebService>();

    public string[] GetUniqueNames()
        => _webServiceUniqueNames ??= Get().Select(x => x.UniqueName).ToArray();
}