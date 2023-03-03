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

namespace Ticker.TickerMessageResource;

public sealed class Get : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Get(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Get_ShouldSucceed_WithoutCustomProperties()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("priority", existingTickerMessage.Priority), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("snowflake", existingTickerMessage.Snowflake), (x.Key, (long?)x.Value)),
            x => Assert.Equal(("text", existingTickerMessage.Text), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("tickerUser", existingTickerMessage.TickerUser), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("timestamp", existingTickerMessage.Timestamp), (x.Key, (long?)x.Value)));
    }

    [Fact]
    public async Task Get_ShouldFail_WhenAbsent()
    {
        // Arrange
        var absentTickerMessage = 1;

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/ticker/ticker-messages/{absentTickerMessage}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Get_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidTickerMessage = "invalid";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/ticker/ticker-messages/{invalidTickerMessage}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}