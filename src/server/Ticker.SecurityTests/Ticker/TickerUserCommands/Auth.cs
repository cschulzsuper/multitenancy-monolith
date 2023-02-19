using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.TickerUserCommands;

public sealed class Auth : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public Auth(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Auth_ShouldSucceed_WhenValid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");

        var authRequest = new
        {
            Client = MockWebApplication.Client,
            Group = MockWebApplication.Group,
            Mail = MockWebApplication.TickerUserMailAddress,
            Secret = MockWebApplication.TickerUserSecret
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Auth_ShouldBeForbidden_WhenClientAbsent()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");

        var authRequest = new
        {
            Client = "absent",
            Group = MockWebApplication.Group,
            Mail = MockWebApplication.TickerUserMailAddress,
            Secret = MockWebApplication.TickerUserSecret
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Auth_ShouldBeForbidden_WhenGroupAbsent()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");

        var authRequest = new
        {
            MockWebApplication.Client,
            Group = "absent",
            Mail = MockWebApplication.TickerUserMailAddress,
            Secret = MockWebApplication.TickerUserSecret
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Auth_ShouldBeForbidden_WhenTickerUserAbsent()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");

        var authRequest = new
        {
            MockWebApplication.Client,
            MockWebApplication.Group,
            Mail = "absent@localhost.com",
            Secret = MockWebApplication.TickerUserSecret
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Auth_ShouldBeForbidden_WhenSecretIncorrect()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");

        var authRequest = new
        {
            MockWebApplication.Client,
            MockWebApplication.Group,
            Mail = MockWebApplication.TickerUserMailAddress,
            Secret = "inavlid"
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