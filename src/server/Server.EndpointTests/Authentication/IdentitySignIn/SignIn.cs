using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Authentication.IdentitySignIn;

public sealed class SignIn : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public SignIn(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.AdminSecret)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.GuestSecret)]
    public async Task SignIn_ShouldSucceed_WhenCredentialAreValid(string identity, string secret)
    {
        // Arrange
        var client = _factory.CreateClient();

        var requestUrl = $"/identities/{identity}/sign-in";
        var requestBody = new { Secret = secret };

        // Act
        var response = await client.PostAsJsonAsync(requestUrl, requestBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, "invalid")]
    [InlineData(TestConfiguration.GuestIdentity, "invalid")]
    public async Task SignIn_ShouldFail_WhenCredentialAreInvalid(string identity, string secret)
    {
        // Arrange
        var client = _factory.CreateClient();

        var requestUrl = $"/identities/{identity}/sign-in";
        var requestBody = new { Secret = secret };

        // Act
        var response = await client.PostAsJsonAsync(requestUrl, requestBody);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }
}