using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Access.AccountGroupResource;

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
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/access/account-groups");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAccountGroup = new
        {
            UniqueName = $"post-account-group-{Guid.NewGuid()}"
        };

        request.Content = JsonContent.Create(postAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("uniqueName", postAccountGroup.UniqueName), (x.Key, (string?)x.Value)));

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountGroup>>()
            .GetQueryable()
            .SingleOrDefault(x =>
                x.UniqueName == postAccountGroup.UniqueName);

        Assert.NotNull(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameExists()
    {
        // Arrange
        var existingAccountGroup = new AccountGroup
        {
            UniqueName = $"existing-account-group-{Guid.NewGuid()}"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountGroup>>()
                .Insert(existingAccountGroup);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/access/account-groups");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAccountGroup = new
        {
            existingAccountGroup.UniqueName
        };

        request.Content = JsonContent.Create(postAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateScope())
        {
            var unchangedIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountGroup>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountGroup.Snowflake &&
                    x.UniqueName == existingAccountGroup.UniqueName);

            Assert.NotNull(unchangedIdentity);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/access/account-groups");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAccountGroup = new
        {
            UniqueName = (string?)null
        };

        request.Content = JsonContent.Create(postAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountGroup>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountGroup.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/access/account-groups");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAccountGroup = new
        {
            UniqueName = string.Empty
        };

        request.Content = JsonContent.Create(postAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountGroup>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountGroup.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/access/account-groups");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAccountGroup = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray())
        };

        request.Content = JsonContent.Create(postAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountGroup>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountGroup.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/access/account-groups");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAccountGroup = new
        {
            UniqueName = "Invalid"
        };

        request.Content = JsonContent.Create(postAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountGroup>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountGroup.UniqueName);

        Assert.Null(createdIdentity);
    }
}