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

public sealed class InsertFlow : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public InsertFlow(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Execute()
    {
        await TickerMessage_Create_ShouldSucceed();
        await TickerBookmark_Query_ShouldSucceed();
    }

    private async Task TickerMessage_Create_ShouldSucceed()
    {
        // Arrange
        var tickerMessageInsertedTask = new TaskCompletionSource();
        var tickerMessageInsertedTaskCancellationTokenSource = new CancellationTokenSource(5000);
        tickerMessageInsertedTaskCancellationTokenSource.Token.Register(() =>
        {
            if (!tickerMessageInsertedTask.Task.IsCompleted) tickerMessageInsertedTask.SetCanceled();
        });

        var tickerBookmarkInsertedTask = new TaskCompletionSource();
        var tickerBookmarkInsertedTaskCancellationTokenSource = new CancellationTokenSource(5000);
        tickerBookmarkInsertedTaskCancellationTokenSource.Token.Register(() =>
        {
            if (!tickerBookmarkInsertedTask.Task.IsCompleted) tickerBookmarkInsertedTask.SetCanceled();
        });

        _factory.Services.GetRequiredService<EventsOptions>()
            .PublicationInterceptor = (scope, @event, snowflake) =>
            {
                switch (@event)
                {
                    case "ticker-message-inserted":
                        tickerMessageInsertedTask.SetResult();
                        break;
                    case "ticker-bookmark-inserted":
                        tickerBookmarkInsertedTask.SetResult();
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
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

        await tickerBookmarkInsertedTask.Task;
        Assert.True(tickerBookmarkInsertedTask.Task.IsCompletedSuccessfully);
    }

    private async Task TickerBookmark_Query_ShouldSucceed()
    {
        // Arrange
        var tickerBookmarkInsertedTask = new TaskCompletionSource();
        var tickerBookmarkInsertedTaskCancellationTokenSource = new CancellationTokenSource(5000);
        tickerBookmarkInsertedTaskCancellationTokenSource.Token.Register(() => { if (!tickerBookmarkInsertedTask.Task.IsCompleted) tickerBookmarkInsertedTask.SetCanceled(); });

        _factory.Services.GetRequiredService<EventsOptions>()
            .PublicationInterceptor = (scope, @event, snowflake) =>
            {
                Assert.Equal("ticker-bookmark-inserted", @event);
                tickerBookmarkInsertedTask.SetResult();
            };

        var queryRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/ticker/ticker-users/_/bookmarks");
        queryRequest.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(queryRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("tickerMessage")),
            x => Assert.True(x.GetProperty("updated").GetBoolean()));
    }
}