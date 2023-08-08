using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Web;

internal sealed class WebServiceClient : IWebServiceClient
{
    private readonly HttpClient _httpClient;

    public WebServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    public WebServiceStatusCodeResult TryGet(string path, int attempts)
    {
        if (_httpClient.BaseAddress == null)
        {
            throw new UnreachableException("Base address is null");
        }

        foreach (var _ in Enumerable.Range(0, attempts))
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, path);
                using var response = _httpClient.Send(request, HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                    return WebServiceStatusCodeResult.Success(_httpClient.BaseAddress, path);
                }
            }
            catch (TaskCanceledException)
            {
                continue;
            }
        }

        return WebServiceStatusCodeResult.Failed(_httpClient.BaseAddress, path);
    }
}