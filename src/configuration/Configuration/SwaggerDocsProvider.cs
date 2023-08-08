using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public sealed class SwaggerDocsProvider : ISwaggerDocsProvider
{
    private readonly IConfiguration _configuration;

    public SwaggerDocsProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ICollection<SwaggerDoc> Get()
        => _configuration.GetSection("SwaggerDocs").Get<SwaggerDoc[]>() ?? Array.Empty<SwaggerDoc>();
}