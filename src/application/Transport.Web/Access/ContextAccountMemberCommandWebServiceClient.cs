using ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;
using ChristianSchulz.MultitenancyMonolith.Web;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal sealed class ContextAccountMemberCommandWebServiceClient : IContextAccountMemberCommandClient
{
    private readonly IWebServiceClient _client;

    public ContextAccountMemberCommandWebServiceClient(IWebServiceClient client)
    {
        _client = client;
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public async Task<object> AuthAsync(ContextAccountMemberAuthCommand command)
    {
        var request = new AnonymousHttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"api/a1/access/account-members/_/auth", UriKind.Relative),
            Content = JsonContent.Create(command),
        };

        var response = await _client.SendAsync<JsonObject>(request);

        var responseAccessToken = response["access_token"]?.GetValue<string>();
        if (responseAccessToken == null)
        {
            TransportException.ThrowProcessViolation("Response does not contain a valid 'access_token'.");
        }

        return responseAccessToken;
    }

    public async Task VerifyAsync()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"api/a1/access/account-members/_/verify", UriKind.Relative),
        };

        await _client.SendAsync(request);
    }
}