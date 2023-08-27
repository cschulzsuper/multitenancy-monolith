using ChristianSchulz.MultitenancyMonolith.Web;
using System;
using System.Net.Http;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Swagger.SwaggerUI;

internal sealed class SwaggerJsonClient : IDisposable
{
    private readonly IWebServiceClient _client;

    public SwaggerJsonClient(IWebServiceClient client)
    {
        _client = client;
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public bool Test(string path)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(path, UriKind.Relative)
        };

        var result = _client.Test(request);

        return result.IsSuccessStatusCode;
    }
}
