using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Authentication.IdentityCommands;

public sealed class IdentitySignInTests : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public IdentitySignInTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Verfiy_ShouldSucceed_WhenAuthorizationIsValid(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/authentication/identities/me/verify");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Verfiy_ShouldBeUnauthorized_WhenAuthorizationIsInvalid(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/authentication/identities/me/verify");
        request.Headers.Authorization = _factory.MockInvalidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}