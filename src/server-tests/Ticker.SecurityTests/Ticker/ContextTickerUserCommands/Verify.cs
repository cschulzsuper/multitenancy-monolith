using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.ContextTickerUserCommands;

public sealed class Verify : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Verify(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Verify_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/b1/ticker/ticker-users/_/verify");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockTicker)]
    public async Task Verify_ShouldSucceed_WhenValid(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/b1/ticker/ticker-users/_/verify");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Verify_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/b1/ticker/ticker-users/_/verify");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Fact]
    public async Task Verify_ShouldBeUnauthorized_WhenInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/b1/ticker/ticker-users/_/verify");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}