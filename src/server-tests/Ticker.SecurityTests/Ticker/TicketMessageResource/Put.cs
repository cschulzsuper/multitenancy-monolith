using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Ticker.TicketMessageResource;

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Put_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var validTickerMessage = 1;

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{validTickerMessage}");
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockMember)]
    public async Task Put_ShouldFail_WhenAuthorized(int mock)
    {
        // Arrange
        var validTickerMessage = 1;

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{validTickerMessage}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockTicker)]
    public async Task Put_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var validTickerMessage = 1;

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{validTickerMessage}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);
        request.Content = JsonContent.Create(new object());

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
    public async Task Post_ShouldBeForbidden_WhenInvalid(int mock)
    {
        // Arrange
        var validTickerMessage = 1;

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{validTickerMessage}");
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