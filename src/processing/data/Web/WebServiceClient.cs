using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Web;

internal sealed class WebServiceClient : IWebServiceClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public static Func<Task<string?>> DefaultTokenProvider = () => Task.FromResult((string?)null);
    
    private readonly HttpClient _client;

    private readonly Func<Task<string?>> _tokenProvider;

    public WebServiceClient(HttpClient client) : this(client, DefaultTokenProvider) { }

    public WebServiceClient(HttpClient client, Func<Task<string?>> tokenProvider)
    {
        _client = client;
        _tokenProvider = tokenProvider;
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public void Send(HttpRequestMessage request)
    {
        SetAuthorizationHeader(request);

        var responseMessage = _client.Send(request, HttpCompletionOption.ResponseHeadersRead);

        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task SendAsync(HttpRequestMessage request)
    {
        await SetAuthorizationHeaderAsync(request);

        var responseMessage = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task<TResponse> SendAsync<TResponse>(HttpRequestMessage request)
    {
        await SetAuthorizationHeaderAsync(request);

        var responseMessage = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        responseMessage.EnsureSuccessStatusCode();

        var response = await responseMessage.Content.ReadFromJsonAsync<TResponse>();
        if (response == null)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));
        }

        return response;
    }

    public async IAsyncEnumerable<TResponse> Stream<TResponse>(HttpRequestMessage request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await SetAuthorizationHeaderAsync(request);

        var responseMessage = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        responseMessage.EnsureSuccessStatusCode();

        var responseStream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
        var response = JsonSerializer.DeserializeAsyncEnumerable<TResponse>(responseStream, JsonSerializerOptions, cancellationToken);

        await foreach (var responseItem in response
            .WithCancellation(cancellationToken))
        {
            yield return responseItem!;
        }
    }

    public WebServiceStatusCodeResult Test(HttpRequestMessage request)
    {
        if (_client.BaseAddress == null)
        {
            throw new UnreachableException("Client base address must not be null");
        }

        if (request.RequestUri == null)
        {
            throw new UnreachableException("Request URI must not be null");
        }

        try
        {
            SetAuthorizationHeader(request);

            // TODO Service Discovery does not work with sync request. Need to fallback to SendAsync and Result
            using var response = _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;

            if (!response.IsSuccessStatusCode)
            {
                return WebServiceStatusCodeResult.Failed(_client.BaseAddress, request.RequestUri);
            }
        }
        catch (TaskCanceledException)
        {
            return WebServiceStatusCodeResult.Failed(_client.BaseAddress, request.RequestUri);
        }

        return WebServiceStatusCodeResult.Success(_client.BaseAddress, request.RequestUri);
    }

    public async Task<WebServiceStatusCodeResult> TestAsync(HttpRequestMessage request)
    {
        if (_client.BaseAddress == null)
        {
            throw new UnreachableException("Client base address must not be null");
        }

        if (request.RequestUri == null)
        {
            throw new UnreachableException("Request URI must not be null");
        }

        try
        {
            await SetAuthorizationHeaderAsync(request);

            using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                return WebServiceStatusCodeResult.Failed(_client.BaseAddress, request.RequestUri);
            }
        }
        catch (TaskCanceledException)
        {
            return WebServiceStatusCodeResult.Failed(_client.BaseAddress, request.RequestUri);
        }

        return WebServiceStatusCodeResult.Success(_client.BaseAddress, request.RequestUri);
    }

    private void SetAuthorizationHeader(HttpRequestMessage request)
    {
        if (request is AnonymousHttpRequestMessage)
        {
            return;
        }

        var token = _tokenProvider.Invoke().ConfigureAwait(false).GetAwaiter().GetResult();
        if (token == null)
        {
            return;
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task SetAuthorizationHeaderAsync(HttpRequestMessage request)
    {
        if (request is AnonymousHttpRequestMessage)
        {
            return;
        }

        var token = await _tokenProvider.Invoke();
        if (token == null)
        {
            return;
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

}