using ChristianSchulz.MultitenancyMonolith.Web;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application;

public sealed class TransportWebServiceClientFactory
{
    private readonly IWebServiceClientFactory _webServiceClientFactory;

    public TransportWebServiceClientFactory(IWebServiceClientFactory webServiceClientFactory)
    {
        _webServiceClientFactory = webServiceClientFactory;
    }

    public TClient Create<TClient>(string webService)
        where TClient : IDisposable
    {
        var client = _webServiceClientFactory.Create(webService);

        var typeClient = TransportWebServiceClientMappings.Mappings[typeof(TClient)];

        return (TClient)Activator.CreateInstance(typeClient, client)!;
    }

    public T Create<T>(string webService, Func<Task<string?>> tokenProvider)
        where T : IDisposable
    {
        var client = _webServiceClientFactory.Create(webService, tokenProvider);

        var typeClient = TransportWebServiceClientMappings.Mappings[typeof(T)];

        return (T)Activator.CreateInstance(typeClient, client)!;
    }
}
