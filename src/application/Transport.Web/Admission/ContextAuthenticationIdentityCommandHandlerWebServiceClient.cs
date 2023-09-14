using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Web;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class ContextAuthenticationIdentityCommandHandlerWebServiceClient : IContextAuthenticationIdentityCommandClient
{
    private readonly IWebServiceClient _client;

    public ContextAuthenticationIdentityCommandHandlerWebServiceClient(IWebServiceClient client)
    {
        _client = client;
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public async Task<object> AuthAsync(ContextAuthenticationIdentityAuthCommand command)
    {
        var request = new AnonymousHttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"api/a1/admission/authentication-identities/_/auth", UriKind.Relative),
            Content = JsonContent.Create(command),
        };
       
        var response = await _client.SendAsync<JsonObject>(request);
        var responseAccessToken = response["accessToken"]?.GetValue<string>();

        if (responseAccessToken == null)
        {
            TransportException.ThrowProcessViolation("Response does not contain a valid 'accessToken'.");
        }

        return responseAccessToken;
    }

    public async Task VerifyAsync()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"api/a1/admission/authentication-identities/_/verify", UriKind.Relative),
        };

        await _client.SendAsync(request);
    }
}