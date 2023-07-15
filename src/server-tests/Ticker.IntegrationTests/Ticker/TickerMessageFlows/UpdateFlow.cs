using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace Ticker.TickerMessageFlows;

public sealed class UpdateFlow : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UpdateFlow(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Execute()
    {
        var tickerMessage = await TickerMessage_Create_ShouldSucceed();

        await TickerBookmark_Update_ShouldSucceed(tickerMessage);
        await TickerMessage_Update_ShouldSucceed(tickerMessage);
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

    private async Task TickerBookmark_Update_ShouldSucceed(long tickerMessage)
    {
        // TODO Replace with API call once available

        using var scope = _factory.CreateMultitenancyScope();

        await scope.ServiceProvider
            .GetRequiredService<IRepository<TickerBookmark>>()
            .UpdateOrThrowAsync(
                x => x.TickerMessage == tickerMessage, 
                x => x.Updated = false);
    }

    private async Task TickerMessage_Update_ShouldSucceed(long tickerMessage)
    {
        // Arrange
        var tickerMessageUpdatedTask = new TaskCompletionSource();
        var tickerMessageUpdatedTaskCancellationTokenSource = new CancellationTokenSource(5000);
        tickerMessageUpdatedTaskCancellationTokenSource.Token.Register(() => {
            if (!tickerMessageUpdatedTask.Task.IsCompleted) tickerMessageUpdatedTask.SetCanceled();
        });

        var tickerBookmarkUpdatedTask = new TaskCompletionSource();
        var tickerBookmarkUpdatedTaskCancellationTokenSource = new CancellationTokenSource(5000);
        tickerBookmarkUpdatedTaskCancellationTokenSource.Token.Register(() => {
            if (!tickerBookmarkUpdatedTask.Task.IsCompleted) tickerBookmarkUpdatedTask.SetCanceled();
        });

        _factory.Services.GetRequiredService<EventsOptions>()
            .PublicationInterceptor = (scope, @event, snowflake) =>
            {
                switch (@event)
                {
                    case "ticker-message-updated":
                        tickerMessageUpdatedTask.SetResult();
                        break;
                    case "ticker-bookmark-updated":
                        tickerBookmarkUpdatedTask.SetResult();
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            };

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{tickerMessage}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = MockWebApplication.TickerUserMail
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await tickerMessageUpdatedTask.Task;
        Assert.True(tickerMessageUpdatedTask.Task.IsCompletedSuccessfully);

        await tickerBookmarkUpdatedTask.Task;
        Assert.True(tickerBookmarkUpdatedTask.Task.IsCompletedSuccessfully);
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
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("tickerMessage")),
            x => Assert.True(x.GetProperty("updated").GetBoolean()));
    }

}
