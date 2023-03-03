using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Linq;
using ChristianSchulz.MultitenancyMonolith.Server;

namespace Authorization.MemberResource;

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
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authorization/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postMember = new
        {
            UniqueName = $"post-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("mailAddress", postMember.MailAddress), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", postMember.UniqueName), (x.Key, (string?)x.Value)));

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var createdMember = scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .GetQueryable()
                .SingleOrDefault(x => x.UniqueName == postMember.UniqueName);

            Assert.NotNull(createdMember);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameExists()
    {
        // Arrange
        var existingMember = new Member
        {
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(existingMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authorization/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postMember = new
        {
            UniqueName = existingMember.UniqueName,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var unchangedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingMember.Snowflake &&
                    x.UniqueName == existingMember.UniqueName);

            Assert.NotNull(unchangedMember);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameIsNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authorization/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postMember = new
        {
            UniqueName = (string?)null,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameIsEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authorization/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postMember = new
        {
            UniqueName = string.Empty,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authorization/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postMember = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authorization/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postMember = new
        {
            UniqueName = "Invalid",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authorization/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postMember = new
        {
            UniqueName = "post-member",
            MailAddress = (string?)null
        };

        request.Content = JsonContent.Create(postMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authorization/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postMember = new
        {
            UniqueName = "post-member",
            MailAddress = string.Empty
        };

        request.Content = JsonContent.Create(postMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authorization/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postMember = new
        {
            UniqueName = "post-member",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(postMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressLocalPartTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authorization/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postMember = new
        {
            UniqueName = "post-member",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(postMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authorization/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postMember = new
        {
            UniqueName = "post-member",
            MailAddress = "foo-bar"
        };

        request.Content = JsonContent.Create(postMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postMember.UniqueName);

        Assert.Null(createdMember);
    }
}