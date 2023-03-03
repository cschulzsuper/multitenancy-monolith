using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Ticker.TickerUserResource;

public sealed class Delete : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Delete(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Delete_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var validTickerUser = 1;

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/ticker/ticker-users/{validTickerUser}");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockMember)]
    public async Task Delete_ShouldFail_WhenAuthorized(int mock)
    {
        // Arrange
        var validTickerUser = 1;

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/ticker/ticker-users/{validTickerUser}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData(MockWebApplication.MockTicker)]
    public async Task Delete_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var validTickerUser = 1;

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/ticker/ticker-users/{validTickerUser}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockTicker)]
    public async Task Delete_ShouldBeUnauthorized_WhenInvalid(int mock)
    {
        // Arrange
        var validTickerUser = 1;

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/ticker/ticker-users/{validTickerUser}");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader(mock);
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}