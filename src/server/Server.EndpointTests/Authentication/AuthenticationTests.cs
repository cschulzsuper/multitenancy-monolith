using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;
using Xunit;

namespace Server.EndpointTests.Authentication;

public class AuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public AuthenticationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("admin", "default")]
    [InlineData("guest", "default")]
    public async Task SignIn_ShouldSucceed_WhenCredentialAreValid(string unqiueName, string secret)
    {
        // Arrange
        var client = _factory.CreateClient();

        var requestUrl = $"/identities/{unqiueName}/sign-in";
        var requestBody = new { Secret = secret };

        // Act
        var response = await client.PostAsJsonAsync(requestUrl, requestBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("admin", "invalid")]
    [InlineData("guest", "invalid")]
    public async Task SignIn_ShouldFail_WhenCredentialAreInvalid(string unqiueName, string secret)
    {
        // Arrange
        var client = _factory.CreateClient();

        var requestUrl = $"/identities/{unqiueName}/sign-in";
        var requestBody = new { Secret = secret };

        // Act
        var response = await client.PostAsJsonAsync(requestUrl, requestBody);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }
}