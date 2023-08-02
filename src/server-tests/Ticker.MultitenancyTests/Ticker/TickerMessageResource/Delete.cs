using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.TickerMessageResource;

public sealed class Delete : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Delete(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Delete_ShouldRespectMultitenancy()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.AccountGroup1))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.AccountGroup2))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(MockWebApplication.AccountGroup2);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.AccountGroup1))
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
        }
    }
}