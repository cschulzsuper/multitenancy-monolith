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

namespace Ticker.TickerUserResource;

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
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/b1/ticker/ticker-users");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(MockWebApplication.AccountGroup1);

        var postTickerUser = new
        {
            DisplayName = "Post Test User",
            MailAddress = $"{Guid.NewGuid()}@localhost",
        };

        request.Content = JsonContent.Create(postTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.AccountGroup1))
        {
            var createdTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(createdTickerUser);
        }

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.AccountGroup2))
        {
            var createdTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.Null(createdTickerUser);
        }
    }
}