using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Web;

public interface IWebServiceClient : IDisposable
{
    WebServiceStatusCodeResult Test(HttpRequestMessage request);

    Task<WebServiceStatusCodeResult> TestAsync(HttpRequestMessage request);

    void Send(HttpRequestMessage request);

    Task SendAsync(HttpRequestMessage request);

    Task<TResponse> SendAsync<TResponse>(HttpRequestMessage request);
 
    IAsyncEnumerable<TResponse> Stream<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken = default);
}