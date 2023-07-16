using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ticker.ContextTickerUserBookmarkCommands;

public sealed class Confirm : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Confirm(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Confirm_ShouldSucceed_WhenExists()
    {
        // Arrange
        var existingTickerBookmark = new TickerBookmark
        {
            TickerMessage = 1,
            Updated = true,
            TickerUser = MockWebApplication.Mail,
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerBookmark>>()
                .Insert(existingTickerBookmark);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/_/bookmarks/{existingTickerBookmark.TickerMessage}/confirm");
        request.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerBookmark = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerBookmark>>()
                .GetQueryable()
                .Single();

            Assert.False(updatedTickerBookmark.Updated);
        }
    }

    [Fact]
    public async Task Confirm_ShouldFail_WhenAbsent()
    {
        // Arrange
        var absentTickerBookmark = 1;

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/_/bookmarks/{absentTickerBookmark}/confirm");
        request.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Confirm_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidTickerBookmark = "invalid";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/_/bookmarks/{invalidTickerBookmark}/confirm");
        request.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}