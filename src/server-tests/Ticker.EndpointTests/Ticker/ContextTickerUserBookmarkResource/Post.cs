using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.ContextTickerUserBookmarkResource;

public sealed class Post : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Post(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Post_ShouldSucceed_WhenValid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-users/_/bookmarks");
        request.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader();

        var postTickerBookmark = new
        {
            TickerMessage = 1
        };

        request.Content = JsonContent.Create(postTickerBookmark);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("tickerMessage", postTickerBookmark.TickerMessage), (x.Key, (long?)x.Value)),
            x => Assert.Equal(("updated", false), (x.Key, (bool?)x.Value)));

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var createdTickerBookmark = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerBookmark>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(createdTickerBookmark);
            Assert.Equal(postTickerBookmark.TickerMessage, createdTickerBookmark.TickerMessage);
            Assert.Equal(MockWebApplication.Mail, createdTickerBookmark.TickerUser);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenTickerMessageZero()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-users/_/bookmarks");
        request.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader();

        var postTickerBookmark = new
        {
            TickerMessage = 0
        };

        request.Content = JsonContent.Create(postTickerBookmark);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerBookmark = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerBookmark>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerBookmark);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenTickerMessageNegative()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-users/_/bookmarks");
        request.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader();

        var postTickerBookmark = new
        {
            TickerMessage = -1
        };

        request.Content = JsonContent.Create(postTickerBookmark);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerBookmark = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerBookmark>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerBookmark);
    }
}