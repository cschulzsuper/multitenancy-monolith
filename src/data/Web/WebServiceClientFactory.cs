using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Web;

internal sealed class WebServiceClientFactory : IWebServiceClientFactory
{
    private readonly IHttpClientFactory _clientFactory;

    public WebServiceClientFactory(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public IWebServiceClient Create(string service)
    {
        var httpClient = _clientFactory.CreateClient(service);

        var client = new WebServiceClient(httpClient);

        return client;
    }

    public IWebServiceClient Create(string webService, string token)
        => Create(webService, () => Task.FromResult(token)!);

    public IWebServiceClient Create(string webService, Func<Task<string?>> tokenProvider)
    {
        var httpClient = _clientFactory.CreateClient(webService);

        var client = new WebServiceClient(httpClient, tokenProvider);

        return client;
    }
}
