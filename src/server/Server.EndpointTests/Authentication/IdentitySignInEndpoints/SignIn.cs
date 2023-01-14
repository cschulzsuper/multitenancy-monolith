using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Authentication.IdentitySignInEndpoints;

public sealed class SignIn : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public SignIn(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.AdminSecret)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.GuestSecret)]
    public async Task SignIn_ShouldSucceed_WhenCredentialAreValid(string identity, string secret)
    {
        // Arrange
        var client = _factory.CreateClient();

        var requestUrl = $"/identities/{identity}/sign-in";
        var requestBody = new { Secret = secret, Client = TestConfiguration.ClientName };

        // Act
        var response = await client.PostAsJsonAsync(requestUrl, requestBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity, "invalid")]
    [InlineData(TestConfiguration.GuestIdentity, "invalid")]
    public async Task SignIn_ShouldFail_WhenCredentialAreInvalid(string identity, string secret)
    {
        // Arrange
        var client = _factory.CreateClient();

        var requestUrl = $"/identities/{identity}/sign-in";
        var requestBody = new { Secret = secret, Client = TestConfiguration.ClientName };

        // Act
        var response = await client.PostAsJsonAsync(requestUrl, requestBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.AdminSecret)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.GuestSecret)]
    public async Task SignIn_ShouldFail_WhenClientIsInvalid(string identity, string secret)
    {
        // Arrange
        var client = _factory.CreateClient();

        var requestUrl = $"/identities/{identity}/sign-in";
        var requestBody = new { Secret = secret, Client = "invalid" };

        // Act
        var response = await client.PostAsJsonAsync(requestUrl, requestBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}