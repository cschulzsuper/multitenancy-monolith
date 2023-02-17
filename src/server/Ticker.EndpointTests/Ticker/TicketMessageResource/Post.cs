using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ticker.TicketMessageResource;

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
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message-{Guid.NewGuid()}",
            Priority = "low",
            Object = "business-objects/default",
            TickerUser = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("object", postTickerMessage.Object), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("objectDisplayName", string.Empty), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("priority", postTickerMessage.Priority), (x.Key, (string?)x.Value)),
            x => Assert.Equal("snowflake", x.Key),
            x => Assert.Equal(("text", postTickerMessage.Text), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("tickerUser", postTickerMessage.TickerUser), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("tickerUserDisplayName", string.Empty), (x.Key, (string?)x.Value)),
            x => Assert.Equal("timestamp", x.Key));

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var createdBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Priority == postTickerMessage.Priority &&
                    x.Text == postTickerMessage.Text &&
                    x.Object == postTickerMessage.Object);

            Assert.NotNull(createdBusinessObject);
        }
    }
}