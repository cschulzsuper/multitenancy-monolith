using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ticker.TicketMessageResource;

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
        var existingTickerMessage1 = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-1-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        var existingTickerMessage2 = new TickerMessage
        {
            Snowflake = 2,
            Text = $"existing-ticker-message-2-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage1, existingTickerMessage2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("text")),
            x => {
                Assert.Equal(existingTickerMessage1.Snowflake, x.GetProperty("snowflake").GetInt64());
                Assert.Equal(existingTickerMessage1.Text, x.GetString("text"));
                Assert.Equal(existingTickerMessage1.Priority, x.GetString("priority"));
                Assert.Equal(existingTickerMessage1.TickerUser, x.GetString("tickerUser"));
                Assert.Equal(existingTickerMessage1.Timestamp, x.GetProperty("timestamp").GetInt64());
            },
            x => {
                Assert.Equal(existingTickerMessage2.Snowflake, x.GetProperty("snowflake").GetInt64());
                Assert.Equal(existingTickerMessage2.Text, x.GetString("text"));
                Assert.Equal(existingTickerMessage2.Priority, x.GetString("priority"));
                Assert.Equal(existingTickerMessage2.TickerUser, x.GetString("tickerUser"));
                Assert.Equal(existingTickerMessage2.Timestamp, x.GetProperty("timestamp").GetInt64());
            });
    }
}