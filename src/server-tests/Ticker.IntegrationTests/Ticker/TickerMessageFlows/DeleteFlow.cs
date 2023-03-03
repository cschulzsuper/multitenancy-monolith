﻿using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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

public class DeleteFlow : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private Action<string>? _eventPublicationInterceptorAssert = null;
    private TaskCompletionSource _eventPublicationInterceptorTask = new TaskCompletionSource();

    public DeleteFlow(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
        _factory.Services.GetRequiredService<EventsOptions>()
            .PublicationInterceptor = (scope, @event, snowflake) =>
            {
                _eventPublicationInterceptorAssert?.Invoke(@event);
                _eventPublicationInterceptorTask.SetResult();
            };
    }

    [Fact]
    public async Task Exceute()
    {
        var tickerMessage = await TickerMessage_Create_ShouldSucceed();

        await TickerMessage_Delete_ShouldSucceed(tickerMessage);
        await TickerBookmark_Query_ShouldSucceed();
    }

    private async Task<long> TickerMessage_Create_ShouldSucceed()
    {
        // Arrange
        _eventPublicationInterceptorAssert = @event => Assert.Equal("ticker-message-inserted", @event);
        _eventPublicationInterceptorTask = new TaskCompletionSource();

        var cancellationTokenSource = new CancellationTokenSource(2000);
        cancellationTokenSource.Token.Register(_eventPublicationInterceptorTask.SetCanceled);

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

        await _eventPublicationInterceptorTask.Task;

        return createdTickerUser.Snowflake;
    }
    private async Task TickerMessage_Delete_ShouldSucceed(long tickerMessage)
    {
        // Arrange
        _eventPublicationInterceptorAssert = @event => Assert.Equal("ticker-message-deleted", @event);
        _eventPublicationInterceptorTask = new TaskCompletionSource();

        var cancellationTokenSource = new CancellationTokenSource(2000);
        cancellationTokenSource.Token.Register(_eventPublicationInterceptorTask.SetCanceled);

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/ticker/ticker-messages/{tickerMessage}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await _eventPublicationInterceptorTask.Task;
    }

    private async Task TickerBookmark_Query_ShouldSucceed()
    {
        // Arrange
        var resetRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/ticker/ticker-users/me/bookmarks");
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