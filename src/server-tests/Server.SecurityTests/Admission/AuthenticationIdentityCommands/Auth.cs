using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Admission.AuthenticationIdentityCommands;

public sealed class Auth : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public Auth(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Theory]
    [InlineData(MockWebApplication.AuthenticationIdentityAdmin, MockWebApplication.AuthenticationIdentityAdminSecret)]
    [InlineData(MockWebApplication.AuthenticationIdentityIdentity, MockWebApplication.AuthenticationIdentityIdentitySecret)]
    [InlineData(MockWebApplication.AuthenticationIdentityDemo, MockWebApplication.AuthenticationIdentityDemoSecret)]
    public async Task Auth_ShouldSucceed_WhenValid(string identity, string secret)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/admission/authentication-identities/me/auth");

        var authRequest = new
        {
            MockWebApplication.Client,
            Identity = identity,
            Secret = secret
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(MockWebApplication.AuthenticationIdentityAdmin)]
    [InlineData(MockWebApplication.AuthenticationIdentityIdentity)]
    [InlineData(MockWebApplication.AuthenticationIdentityDemo)]
    public async Task Auth_ShouldFail_WhenSecretInvalid(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/admission/authentication-identities/me/auth");

        var authRequest = new
        {
            MockWebApplication.Client,
            Identity = identity,
            Secret = "invalid"
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
    [InlineData(MockWebApplication.AuthenticationIdentityAdmin, MockWebApplication.AuthenticationIdentityAdminSecret)]
    [InlineData(MockWebApplication.AuthenticationIdentityIdentity, MockWebApplication.AuthenticationIdentityIdentitySecret)]
    [InlineData(MockWebApplication.AuthenticationIdentityDemo, MockWebApplication.AuthenticationIdentityDemoSecret)]
    public async Task Auth_ShouldFail_WhenClientInvalid(string identity, string secret)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/admission/authentication-identities/me/auth");

        var authRequest = new
        {
            Client = "Invalid",
            Identity = identity,
            Secret = secret
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