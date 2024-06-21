using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
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

public sealed class Post 
{
    [Fact]
    public async Task Post_ShouldSucceed_WhenValid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-groups");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountGroup = new
        {
            UniqueName = $"post-account-group-{Guid.NewGuid()}"
        };

        request.Content = JsonContent.Create(postAccountGroup);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("uniqueName", postAccountGroup.UniqueName), (x.Key, (string?)x.Value)));

        using var scope = application.Services.CreateScope();

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
        using var application = MockWebApplication.Create();

        var existingAccountGroup = new AccountGroup
        {
            UniqueName = $"existing-account-group-{Guid.NewGuid()}"
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountGroup>>()
                .Insert(existingAccountGroup);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-groups");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountGroup = new
        {
            existingAccountGroup.UniqueName
        };

        request.Content = JsonContent.Create(postAccountGroup);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.Services.CreateScope())
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
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-groups");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountGroup = new
        {
            UniqueName = (string?)null
        };

        request.Content = JsonContent.Create(postAccountGroup);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

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
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-groups");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountGroup = new
        {
            UniqueName = string.Empty
        };

        request.Content = JsonContent.Create(postAccountGroup);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

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
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-groups");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountGroup = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray())
        };

        request.Content = JsonContent.Create(postAccountGroup);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

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
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-groups");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountGroup = new
        {
            UniqueName = "Invalid"
        };

        request.Content = JsonContent.Create(postAccountGroup);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountGroup>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountGroup.UniqueName);

        Assert.Null(createdIdentity);
    }
}