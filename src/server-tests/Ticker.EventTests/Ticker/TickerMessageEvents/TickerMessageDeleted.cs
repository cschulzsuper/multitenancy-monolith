using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.Extensions.DependencyInjection;
using ChristianSchulz.MultitenancyMonolith.Events;
using Xunit.Abstractions;

namespace Ticker.TickerMessageEvents;

public sealed class TickerMessageDeleted : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TickerMessageDeleted(
        WebApplicationFactory<Program> factory,
        ITestOutputHelper testOutputHelper)
    {
        _factory = factory.Mock(testOutputHelper);
    }

    [Fact]
    public async Task TickerMessageDeleted_ShouldUpdateBookmarks()
    {
        // Arrange

        await using var scope = _factory.CreateAsyncMultitenancyScope();
        
        var existingTickerBookmark1 = new TickerBookmark
        {
            TickerMessage = 1,
            Updated = false,
            TickerUser = $"{Guid.NewGuid()}@localhost"
        };

        var existingTickerBookmark2 = new TickerBookmark
        {
            TickerMessage = 1,
            Updated = false,
            TickerUser = $"{Guid.NewGuid()}@localhost"
        };

        scope.ServiceProvider
            .GetRequiredService<IRepository<TickerBookmark>>()
            .Insert(existingTickerBookmark1, existingTickerBookmark2);

        // Act
        await scope.ServiceProvider
            .GetRequiredService<IEventSubscriptions>()
            .InvokeAsync("ticker-message-deleted", scope.ServiceProvider, 1);

        // Assert
        var createdBookmarks = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerBookmark>>()
            .GetQueryable()
            .ToArray();

        Assert.Empty(createdBookmarks);
    }
}
