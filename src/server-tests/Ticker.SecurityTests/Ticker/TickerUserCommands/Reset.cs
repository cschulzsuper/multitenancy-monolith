using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.TickerUserCommands;

public sealed class Reset : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Reset(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Reset_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var validTickerUser = 1;

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/b1/ticker/ticker-users/{validTickerUser}/reset");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockChief)]
    public async Task Reset_ShouldFail_WhenAuthorized(int mock)
    {
        // Arrange
        var validTickerUser = 1;

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/b1/ticker/ticker-users/{validTickerUser}/reset");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    [InlineData(MockWebApplication.MockTicker)]
    public async Task Reset_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var validTickerUser = 1;

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/b1/ticker/ticker-users/{validTickerUser}/reset");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    [InlineData(MockWebApplication.MockTicker)]
    public async Task Reset_ShouldBeUnauthorized_WhenInvalid(int mock)
    {
        // Arrange
        var validTickerUser = 1;

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/b1/ticker/ticker-users/{validTickerUser}/reset");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}