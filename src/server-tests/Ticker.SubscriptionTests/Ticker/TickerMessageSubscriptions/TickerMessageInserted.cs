﻿using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ticker.TickerMessageSubscriptions;

public sealed class TickerMessageInserted : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TickerMessageInserted(
        WebApplicationFactory<Program> factory,
        ITestOutputHelper testOutputHelper)
    {
        _factory = factory.Mock(testOutputHelper);
    }

    [Fact]
    public async Task TickerMessageInserted_ShouldCreateBookmark()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = MockWebApplication.Mail,
            Timestamp = 0
        };

        await using var scope = _factory.CreateAsyncMultitenancyScope();

        scope.ServiceProvider
            .GetRequiredService<IRepository<TickerMessage>>()
            .Insert(existingTickerMessage);

        // Act
        await scope.ServiceProvider
            .GetRequiredService<IEventSubscriptions>()
            .InvokeAsync("ticker-message-inserted", scope.ServiceProvider, existingTickerMessage.Snowflake);

        // Assert
        var createdBookmark = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerBookmark>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.NotNull(createdBookmark);
        Assert.Equal(existingTickerMessage.Snowflake, createdBookmark.TickerMessage);
        Assert.Equal(existingTickerMessage.TickerUser, createdBookmark.TickerUser);
        Assert.True(createdBookmark.Updated);
    }
}