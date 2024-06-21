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

namespace Access.AccountMemberResource;

public sealed class Post 
{
    [Fact]
    public async Task Post_ShouldSucceed_WhenValid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-members");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postAccountMember = new
        {
            UniqueName = $"post-account-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("mailAddress", postAccountMember.MailAddress), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", postAccountMember.UniqueName), (x.Key, (string?)x.Value)));

        using (var scope = application.CreateMultitenancyScope())
        {
            var createdMember = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .GetQueryable()
                .SingleOrDefault(x => x.UniqueName == postAccountMember.UniqueName);

            Assert.NotNull(createdMember);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameExists()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountMember = new AccountMember
        {
            UniqueName = $"existing-account-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(existingAccountMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-members");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postAccountMember = new
        {
            existingAccountMember.UniqueName,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.CreateMultitenancyScope())
        {
            var unchangedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountMember.Snowflake &&
                    x.UniqueName == existingAccountMember.UniqueName);

            Assert.NotNull(unchangedMember);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameIsNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-members");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postAccountMember = new
        {
            UniqueName = (string?)null,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountMember>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameIsEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-members");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postAccountMember = new
        {
            UniqueName = string.Empty,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountMember>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-members");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postAccountMember = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountMember>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-members");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postAccountMember = new
        {
            UniqueName = "Invalid",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountMember>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-members");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postAccountMember = new
        {
            UniqueName = "post-account-member",
            MailAddress = (string?)null
        };

        request.Content = JsonContent.Create(postAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountMember>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-members");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postAccountMember = new
        {
            UniqueName = "post-account-member",
            MailAddress = string.Empty
        };

        request.Content = JsonContent.Create(postAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountMember>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-members");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postAccountMember = new
        {
            UniqueName = "post-account-member",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(postAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountMember>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressLocalPartTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-members");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postAccountMember = new
        {
            UniqueName = "post-account-member",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(postAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountMember>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountMember.UniqueName);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-members");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var postAccountMember = new
        {
            UniqueName = "post-account-member",
            MailAddress = "foo-bar"
        };

        request.Content = JsonContent.Create(postAccountMember);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountMember>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAccountMember.UniqueName);

        Assert.Null(createdMember);
    }
}