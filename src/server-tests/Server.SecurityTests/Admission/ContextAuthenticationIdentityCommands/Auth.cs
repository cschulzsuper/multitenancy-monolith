using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Admission.ContextAuthenticationIdentityCommands;

public sealed class Auth : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public Auth(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Theory]
    [InlineData(MockWebApplication.AuthenticationIdentityAdmin, MockWebApplication.AuthenticationIdentityAdminSecret, null)]
    [InlineData(MockWebApplication.AuthenticationIdentityIdentity, MockWebApplication.AuthenticationIdentityIdentitySecret, AuthenticationMethods.Secret)]
    [InlineData(MockWebApplication.AuthenticationIdentityDemo, null, AuthenticationMethods.Anonymouse)]
    public async Task Auth_ShouldSucceed_WhenValid(string authenticationIdentity, string secret, string method)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-identities/_/auth");

        var authRequest = new
        {
            ClientName = MockWebApplication.ClientName,
            AuthenticationIdentity = authenticationIdentity,
            Secret = secret,
            Method = method
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(MockWebApplication.AuthenticationIdentityAdmin, "invalid", null)]
    [InlineData(MockWebApplication.AuthenticationIdentityIdentity, "invalid", AuthenticationMethods.Secret)]
    [InlineData(MockWebApplication.AuthenticationIdentityDemo, null, AuthenticationMethods.Secret)]
    public async Task Auth_ShouldFail_WhenSecretInvalid(string authenticationIdentity, string secret, string method)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-identities/_/auth");

        var authRequest = new
        {
            ClientName = MockWebApplication.ClientName,
            AuthenticationIdentity = authenticationIdentity,
            Secret = secret,
            Method = method
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData(MockWebApplication.AuthenticationIdentityAdmin, MockWebApplication.AuthenticationIdentityAdminSecret, null)]
    [InlineData(MockWebApplication.AuthenticationIdentityIdentity, MockWebApplication.AuthenticationIdentityIdentitySecret, AuthenticationMethods.Secret)]
    [InlineData(MockWebApplication.AuthenticationIdentityDemo, null, AuthenticationMethods.Anonymouse)]
    public async Task Auth_ShouldFail_WhenClientInvalid(string authenticationIdentity, string secret, string method)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-identities/_/auth");

        var authRequest = new
        {
            ClientName = "Invalid",
            AuthenticationIdentity = authenticationIdentity,
            Secret = secret,
            Method = method
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}