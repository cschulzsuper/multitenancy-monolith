using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
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

namespace Authentication.IdentityResource;

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
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            UniqueName = $"post-identity-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("mailAddress", postIdentity.MailAddress), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", postIdentity.UniqueName), (x.Key, (string?)x.Value)));

        using (var scope = _factory.Services.CreateScope())
        {
            var createdIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.UniqueName == postIdentity.UniqueName &&
                    x.MailAddress == postIdentity.MailAddress &&
                    x.Secret == postIdentity.Secret);

            Assert.NotNull(createdIdentity);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameExists()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            existingIdentity.UniqueName,
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateScope())
        {
            var unchangedIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingIdentity.Snowflake &&
                    x.UniqueName == existingIdentity.UniqueName &&
                    x.MailAddress == existingIdentity.MailAddress &&
                    x.Secret == existingIdentity.Secret);

            Assert.NotNull(unchangedIdentity);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            UniqueName = (string?)null,
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            UniqueName = string.Empty,
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            UniqueName = "Invalid",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenSecretNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            UniqueName = "post-identity",
            MailAddress = "info@localhost",
            Secret = (string?)null,
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenSecretEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            UniqueName = "post-identity",
            MailAddress = "info@localhost",
            Secret = string.Empty
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenSecretTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            UniqueName = "post-identity",
            MailAddress = "info@localhost",
            Secret = new string(Enumerable.Repeat('a', 141).ToArray())
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            UniqueName = "post-identity",
            MailAddress = (string?)null,
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            UniqueName = "post-identity",
            MailAddress = string.Empty,
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            UniqueName = "post-identity",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressLocalPartTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            UniqueName = "post-identity",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postIdentity = new
        {
            UniqueName = "post-identity",
            MailAddress = "foo-bar",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }
}