﻿using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.ContextTickerUserBookmarkResource;

public sealed class Delete : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Delete(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Delete_ShouldRespectMultitenancy_WhenSuccessful()
    {
        // Arrange
        var existingTickerBookmark = new TickerBookmark
        {
            TickerMessage = 1,
            Updated = true,
            TickerUser = MockWebApplication.Mail,
        };

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.AccountGroup1))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerBookmark>>()
                .Insert(existingTickerBookmark);
        }

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.AccountGroup2))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerBookmark>>()
                .Insert(existingTickerBookmark);
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/b1/ticker/ticker-users/_/bookmarks/{existingTickerBookmark.TickerMessage}");
        request.Headers.Authorization = _factory.MockValidTickerAuthorizationHeader(MockWebApplication.AccountGroup2);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.AccountGroup1))
        {
            var deletedTickerBookmark = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerBookmark>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(deletedTickerBookmark);
        }
    }
}