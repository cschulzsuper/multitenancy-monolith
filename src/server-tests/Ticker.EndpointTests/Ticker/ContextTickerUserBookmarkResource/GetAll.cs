using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.ContextTickerUserBookmarkResource;

public sealed class GetAll : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetAll(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task GetAll_ShouldSucceed_WhenValid()
    {
        // Arrange
        var existingTickerBookmark1 = new TickerBookmark
        {
            TickerMessage = 1,
            Updated = true,
            TickerUser = MockWebApplication.Mail,
        };

        var existingTickerBookmark2 = new TickerBookmark
        {
            TickerMessage = 2,
            Updated = true,
            TickerUser = MockWebApplication.Mail,
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerBookmark>>()
                .Insert(existingTickerBookmark1, existingTickerBookmark2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/b1/ticker/ticker-users/_/bookmarks");
        request.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("tickerMessage")),
            x =>
            {
                Assert.Equal(existingTickerBookmark1.Updated, x.GetProperty("updated").GetBoolean());
                Assert.Equal(existingTickerBookmark1.TickerMessage, x.GetProperty("tickerMessage").GetInt64());
            },
            x =>
            {
                Assert.Equal(existingTickerBookmark2.Updated, x.GetProperty("updated").GetBoolean());
                Assert.Equal(existingTickerBookmark2.TickerMessage, x.GetProperty("tickerMessage").GetInt64());
            });
    }
}