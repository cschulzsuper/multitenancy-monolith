using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.TickerMessageFlows;

public sealed class DeleteFlow : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DeleteFlow(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Execute()
    {
        var tickerMessage = await TickerMessage_Create_ShouldSucceed();

        await TickerMessage_Delete_ShouldSucceed(tickerMessage);
        await TickerBookmark_Query_ShouldSucceed();
    }

    private async Task<long> TickerMessage_Create_ShouldSucceed()
    {
        // Arrange
        var tickerMessageInsertedTask = new TaskCompletionSource();
        var tickerMessageInsertedTaskCancellationTokenSource = new CancellationTokenSource(5000);
        tickerMessageInsertedTaskCancellationTokenSource.Token.Register(() => { if (!tickerMessageInsertedTask.Task.IsCompleted) tickerMessageInsertedTask.SetCanceled(); });

        _factory.Services.GetRequiredService<EventsOptions>()
            .PublicationInterceptor = (scope, @event, snowflake) =>
            {
                Assert.Equal("ticker-message-inserted", @event);
                tickerMessageInsertedTask.SetResult();
            };

        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        createRequest.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = MockWebApplication.TickerUserMail
        };

        createRequest.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(createRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerMessage>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.NotNull(createdTickerUser);

        await tickerMessageInsertedTask.Task;
        Assert.True(tickerMessageInsertedTask.Task.IsCompletedSuccessfully);

        return createdTickerUser.Snowflake;
    }
    private async Task TickerMessage_Delete_ShouldSucceed(long tickerMessage)
    {
        // Arrange
        var tickerMessageDeletedTask = new TaskCompletionSource();
        var tickerMessageDeletedTaskCancellationTokenSource = new CancellationTokenSource(5000);
        tickerMessageDeletedTaskCancellationTokenSource.Token.Register(() =>
        {
            if (!tickerMessageDeletedTask.Task.IsCompleted) tickerMessageDeletedTask.SetCanceled();
        });

        var tickerBookmarkDeletedTask = new TaskCompletionSource();
        var tickerBookmarkDeletedTaskCancellationTokenSource = new CancellationTokenSource(5000);
        tickerBookmarkDeletedTaskCancellationTokenSource.Token.Register(() =>
        {
            if (!tickerBookmarkDeletedTask.Task.IsCompleted) tickerBookmarkDeletedTask.SetCanceled();
        });

        _factory.Services.GetRequiredService<EventsOptions>()
            .PublicationInterceptor = (scope, @event, snowflake) =>
            {
                switch (@event)
                {
                    case "ticker-message-deleted":
                        tickerMessageDeletedTask.SetResult();
                        break;
                    case "ticker-bookmark-deleted":
                        tickerBookmarkDeletedTask.SetResult();
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            };

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/ticker/ticker-messages/{tickerMessage}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await tickerMessageDeletedTask.Task;
        Assert.True(tickerMessageDeletedTask.Task.IsCompletedSuccessfully);

        await tickerBookmarkDeletedTask.Task;
        Assert.True(tickerBookmarkDeletedTask.Task.IsCompletedSuccessfully);
    }

    private async Task TickerBookmark_Query_ShouldSucceed()
    {
        // Arrange
        var resetRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/ticker/ticker-users/_/bookmarks");
        resetRequest.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(resetRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Empty(content.RootElement.EnumerateArray());
    }
}