using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
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
    public async Task Post_ShouldSucceed_WhenValid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-users");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("displayName", postTickerUser.DisplayName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("mailAddress", postTickerUser.MailAddress), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("secretState", TickerUserSecretStates.Invalid), (x.Key, (string?)x.Value)),
            x => Assert.Equal("snowflake", x.Key));

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.NotNull(createdTickerUser);
        Assert.Equal(postTickerUser.DisplayName, createdTickerUser.DisplayName);
        Assert.Equal(postTickerUser.MailAddress, createdTickerUser.MailAddress);
        Assert.Equal(TickerUserSecretStates.Invalid, createdTickerUser.SecretState);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenDisplayNameNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-users");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerUser = new
        {
            DisplayName = (string?)null,
            MailAddress = $"{Guid.NewGuid()}@localhost",
        };

        request.Content = JsonContent.Create(postTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerUser);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenDisplayNameEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-users");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerUser = new
        {
            DisplayName = string.Empty,
            MailAddress = $"{Guid.NewGuid()}@localhost",
        };

        request.Content = JsonContent.Create(postTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerUser);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenDisplayNameTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-users");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerUser = new
        {
            DisplayName = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = $"{Guid.NewGuid()}@localhost"
        };

        request.Content = JsonContent.Create(postTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerUser);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-users");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerUser = new
        {
            DisplayName = "Post Test User",
            MailAddress = (string?)null
        };

        request.Content = JsonContent.Create(postTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerUser);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-users");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerUser = new
        {
            DisplayName = "Post Test User",
            MailAddress = string.Empty
        };

        request.Content = JsonContent.Create(postTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerUser);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-users");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerUser = new
        {
            DisplayName = "Post Test User",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(postTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerUser);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressLocalPartTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-users");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerUser = new
        {
            DisplayName = "Post Test User",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(postTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerUser);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticker/ticker-users");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postTickerUser = new
        {
            DisplayName = "Post Test User",
            MailAddress = "Invalid"
        };

        request.Content = JsonContent.Create(postTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdTickerUser);
    }
}