using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

internal sealed class SwaggerDocsProvider : ISwaggerDocsProvider
{
    private readonly IConfiguration _configuration;

    public SwaggerDocsProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ICollection<SwaggerDocs> Get()
        => _configuration.GetSection(nameof(SwaggerDocs)).Get<SwaggerDocs[]>() ?? Array.Empty<SwaggerDocs>();
}
