using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
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

namespace Admission.AuthenticationIdentityResource;

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
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = $"post-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("mailAddress", postAuthenticationIdentity.MailAddress), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", postAuthenticationIdentity.UniqueName), (x.Key, (string?)x.Value)));

        using (var scope = _factory.Services.CreateScope())
        {
            var createdIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.UniqueName == postAuthenticationIdentity.UniqueName &&
                    x.MailAddress == postAuthenticationIdentity.MailAddress &&
                    x.Secret == postAuthenticationIdentity.Secret);

            Assert.NotNull(createdIdentity);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameExists()
    {
        // Arrange
        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            existingAuthenticationIdentity.UniqueName,
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateScope())
        {
            var unchangedIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAuthenticationIdentity.Snowflake &&
                    x.UniqueName == existingAuthenticationIdentity.UniqueName &&
                    x.MailAddress == existingAuthenticationIdentity.MailAddress &&
                    x.Secret == existingAuthenticationIdentity.Secret);

            Assert.NotNull(unchangedIdentity);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = (string?)null,
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAuthenticationIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = string.Empty,
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAuthenticationIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAuthenticationIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "Invalid",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAuthenticationIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenSecretNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "post-authentication-identity",
            MailAddress = "info@localhost",
            Secret = (string?)null,
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAuthenticationIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenSecretEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "post-authentication-identity",
            MailAddress = "info@localhost",
            Secret = string.Empty
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAuthenticationIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenSecretTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "post-authentication-identity",
            MailAddress = "info@localhost",
            Secret = new string(Enumerable.Repeat('a', 141).ToArray())
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAuthenticationIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "post-authentication-identity",
            MailAddress = (string?)null,
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAuthenticationIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "post-authentication-identity",
            MailAddress = string.Empty,
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAuthenticationIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "post-authentication-identity",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAuthenticationIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressLocalPartTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "post-authentication-identity",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAuthenticationIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "post-authentication-identity",
            MailAddress = "foo-bar",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAuthenticationIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }
}