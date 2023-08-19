using ChristianSchulz.MultitenancyMonolith.Web;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger.SwaggerUI;

internal sealed class SwaggerJsonClientFactory
{
    private readonly IWebServiceClientFactory _webServiceClientFactory;
    private readonly SwaggerJsonClientTokenProvider _swaggerJsonClientTokenProvider;

    public SwaggerJsonClientFactory(
        IWebServiceClientFactory webServiceClientFactory,
        SwaggerJsonClientTokenProvider swaggerJsonClientTokenProvider)
    {
        _webServiceClientFactory = webServiceClientFactory;
        _swaggerJsonClientTokenProvider = swaggerJsonClientTokenProvider;
    }

    public SwaggerJsonClient Create(string webService)
    {
        Task<string> tokenProvider() => _swaggerJsonClientTokenProvider.GetAsync();

        var webServiceClient = _webServiceClientFactory.Create(webService, tokenProvider!);

        var swaggerJsonClient = new SwaggerJsonClient(webServiceClient);

        return swaggerJsonClient;
    }
}
