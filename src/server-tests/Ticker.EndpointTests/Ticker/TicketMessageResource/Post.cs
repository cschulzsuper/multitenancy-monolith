using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ticker.TicketMessageResource;

public sealed class Post : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Post(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Post_ShouldSucceed_WhenPriorityLow()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message-{Guid.NewGuid()}",
            Priority = "low",
            TickerUser = $"{Guid.NewGuid()}@localhost"
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("priority", postTickerMessage.Priority), (x.Key, (string?)x.Value)),
            x => Assert.Equal("snowflake", x.Key),
            x => Assert.Equal(("text", postTickerMessage.Text), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("tickerUser", postTickerMessage.TickerUser), (x.Key, (string?)x.Value)),
            x => Assert.Equal("timestamp", x.Key));

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var createdTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(createdTickerMessage);
            Assert.Equal(postTickerMessage.TickerUser, createdTickerMessage.TickerUser);
            Assert.Equal(postTickerMessage.Text, createdTickerMessage.Text);
            Assert.Equal(postTickerMessage.Priority, createdTickerMessage.Priority);
        }
    }

    [Fact]
    public async Task Post_ShouldSucceed_WhenPriorityDefault()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost"
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("priority", postTickerMessage.Priority), (x.Key, (string?)x.Value)),
            x => Assert.Equal("snowflake", x.Key),
            x => Assert.Equal(("text", postTickerMessage.Text), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("tickerUser", postTickerMessage.TickerUser), (x.Key, (string?)x.Value)),
            x => Assert.Equal("timestamp", x.Key));

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var createdTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(createdTickerMessage);
            Assert.Equal(postTickerMessage.TickerUser, createdTickerMessage.TickerUser);
            Assert.Equal(postTickerMessage.Text, createdTickerMessage.Text);
            Assert.Equal(postTickerMessage.Priority, createdTickerMessage.Priority);
        }
    }

    [Fact]
    public async Task Post_ShouldSucceed_WhenPriorityHigh()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message-{Guid.NewGuid()}",
            Priority = "high",
            TickerUser = $"{Guid.NewGuid()}@localhost"
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("priority", postTickerMessage.Priority), (x.Key, (string?)x.Value)),
            x => Assert.Equal("snowflake", x.Key),
            x => Assert.Equal(("text", postTickerMessage.Text), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("tickerUser", postTickerMessage.TickerUser), (x.Key, (string?)x.Value)),
            x => Assert.Equal("timestamp", x.Key));

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var createdTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(createdTickerMessage);
            Assert.Equal(postTickerMessage.TickerUser, createdTickerMessage.TickerUser);
            Assert.Equal(postTickerMessage.Text, createdTickerMessage.Text);
            Assert.Equal(postTickerMessage.Priority, createdTickerMessage.Priority);
        }
    }

    [Fact]
    public async Task Post_ShouldSucceed_WhenPriorityCatastrophe()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message-{Guid.NewGuid()}",
            Priority = "catastrophe",
            TickerUser = $"{Guid.NewGuid()}@localhost"
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("priority", postTickerMessage.Priority), (x.Key, (string?)x.Value)),
            x => Assert.Equal("snowflake", x.Key),
            x => Assert.Equal(("text", postTickerMessage.Text), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("tickerUser", postTickerMessage.TickerUser), (x.Key, (string?)x.Value)),
            x => Assert.Equal("timestamp", x.Key));

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var createdTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(createdTickerMessage);
            Assert.Equal(postTickerMessage.TickerUser, createdTickerMessage.TickerUser);
            Assert.Equal(postTickerMessage.Text, createdTickerMessage.Text);
            Assert.Equal(postTickerMessage.Priority, createdTickerMessage.Priority);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenTextNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = (string?)null,
            Priority = "low",
            TickerUser = $"{Guid.NewGuid()}@localhost"
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerMessage = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerMessage>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerMessage);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenTextEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = string.Empty,
            Priority = "low",
            TickerUser = $"{Guid.NewGuid()}@localhost"
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerMessage = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerMessage>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerMessage);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenTextTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = new string(Enumerable.Repeat('a', 4001).ToArray()),
            Priority = "low",
            TickerUser = $"{Guid.NewGuid()}@localhost"
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerMessage = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerMessage>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerMessage);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenPriorityNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message",
            Priority = (string?)null,
            TickerUser = $"{Guid.NewGuid()}@localhost"
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerMessage = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerMessage>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerMessage);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenPriorityEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message",
            Priority = string.Empty,
            TickerUser = $"{Guid.NewGuid()}@localhost"
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerMessage = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerMessage>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerMessage);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenPriorityInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message",
            Priority = "invalid",
            TickerUser = $"{Guid.NewGuid()}@localhost"
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerMessage = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerMessage>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerMessage);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenTickerUserNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message",
            Priority = "low",
            TickerUser = (string?)null
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerMessage = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerMessage>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerMessage);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenTickerUserEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message",
            Priority = "low",
            TickerUser = string.Empty
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerMessage = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerMessage>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerMessage);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenTickerUserTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message",
            Priority = "low",
            TickerUser = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerMessage = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerMessage>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerMessage);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenTickerUserLocalPartTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message",
            Priority = "low",
            TickerUser = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerMessage = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerMessage>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerMessage);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenTickerUserInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-messages");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerMessage = new
        {
            Text = $"post-ticker-message",
            Priority = "low",
            TickerUser = "Invalid"
        };

        request.Content = JsonContent.Create(postTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerMessage = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerMessage>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerMessage);
    }
}