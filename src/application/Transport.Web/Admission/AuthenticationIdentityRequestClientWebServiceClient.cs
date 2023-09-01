using ChristianSchulz.MultitenancyMonolith.Application.Admission.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Responses;
using ChristianSchulz.MultitenancyMonolith.Web;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class AuthenticationIdentityRequestClientWebServiceClient : IAuthenticationIdentityRequestClient
{
    private readonly IWebServiceClient _client;

    public AuthenticationIdentityRequestClientWebServiceClient(IWebServiceClient client)
    {
        _client = client;
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public Task HeadAsync(string authenticationIdentity)
    {
        throw new NotImplementedException();
    }

    public Task<AuthenticationIdentityResponse> GetAsync(string authenticationIdentity)
    {
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<AuthenticationIdentityResponse> GetAll(string? query, int? skip, int? take)
    {
        var queryStringParameters = new List<KeyValuePair<string, string?>>();

        if (query != null) queryStringParameters.Add(new KeyValuePair<string, string?>("q", query));
        if (skip != null) queryStringParameters.Add(new KeyValuePair<string, string?>("s", $"{skip}"));
        if (take != null) queryStringParameters.Add(new KeyValuePair<string, string?>("t", $"{take}"));

        var queryString = QueryString.Create(queryStringParameters);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"api/a1/admission/authentication-identities{queryString}", UriKind.Relative)
        };

        var response = _client.Stream<AuthenticationIdentityResponse>(request);

        await foreach (var @object in response)
        {
            yield return @object;
        }
    }

    public Task<AuthenticationIdentityResponse> InsertAsync(AuthenticationIdentityRequest request)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(string authenticationIdentity, AuthenticationIdentityRequest request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string authenticationIdentity)
    {
        throw new NotImplementedException();
    }
}