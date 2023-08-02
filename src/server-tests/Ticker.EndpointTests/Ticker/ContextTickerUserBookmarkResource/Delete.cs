using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.ContextTickerUserBookmarkResource;

public sealed class Delete : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Delete(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Delete_ShouldSucceed_WhenExists()
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

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/ticker/ticker-users/_/bookmarks/{existingTickerBookmark.TickerMessage}");
        request.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var deletedTickerBookmark = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerBookmark>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.Null(deletedTickerBookmark);
        }
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenAbsent()
    {
        // Arrange
        var absentTickerBookmark = 1;

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/ticker/ticker-users/_/bookmarks/{absentTickerBookmark}");
        request.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidTickerBookmark = "invalid";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/ticker/ticker-users/_/bookmarks/{invalidTickerBookmark}");
        request.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}