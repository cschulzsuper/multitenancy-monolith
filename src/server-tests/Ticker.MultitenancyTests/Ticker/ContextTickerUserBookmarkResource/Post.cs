using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
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
    public async Task Post_ShouldRespectMultitenancy()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-users/_/bookmarks");
        request.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader(MockWebApplication.AccountGroup1);

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

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.AccountGroup1))
        {
            var createdTickerBookmark = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerBookmark>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(createdTickerBookmark);
        }

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.AccountGroup2))
        {
            var createdTickerBookmark = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerBookmark>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.Null(createdTickerBookmark);
        }
    }
}