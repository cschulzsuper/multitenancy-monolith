using System.Net.Http;

namespace ChristianSchulz.MultitenancyMonolith.Web;

internal class WebServiceClientFactory : IWebServiceClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WebServiceClientFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }


    public IWebServiceClient Create(string uniqueName)
    {
        var httpClient = _httpClientFactory.CreateClient(uniqueName);

        var webServiceClient = new WebServiceClient(httpClient);

        return webServiceClient;
    }
}