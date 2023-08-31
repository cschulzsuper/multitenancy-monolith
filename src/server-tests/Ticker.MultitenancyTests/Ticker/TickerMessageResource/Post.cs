using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.TickerMessageResource;

public sealed class Post : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Post(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Post_ShouldRespectMultitenancy()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/b1/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(MockWebApplication.AccountGroup1);

        var postTickerMessage = new
        {
            Text = $"post-ticker-message-{Guid.NewGuid()}",
            Priority = "low",
            TickerUser = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.AccountGroup1))
        {
            var createdTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(createdTickerMessage);
        }

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.AccountGroup2))
        {
            var createdTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.Null(createdTickerMessage);
        }
    }
}