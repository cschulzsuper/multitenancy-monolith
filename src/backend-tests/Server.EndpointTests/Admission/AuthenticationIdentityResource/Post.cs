using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
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

public sealed class Post 
{
    [Fact]
    public async Task Post_ShouldSucceed_WhenValid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-identities");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = $"post-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("mailAddress", postAuthenticationIdentity.MailAddress), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", postAuthenticationIdentity.UniqueName), (x.Key, (string?)x.Value)));

        using (var scope = application.Services.CreateScope())
        {
            var createdIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.UniqueName == postAuthenticationIdentity.UniqueName &&
                    x.MailAddress == postAuthenticationIdentity.MailAddress);

            Assert.NotNull(createdIdentity);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameExists()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-identities");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            existingAuthenticationIdentity.UniqueName,
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.Services.CreateScope())
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
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-identities");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = (string?)null,
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

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
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-identities");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = string.Empty,
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

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
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-identities");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

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
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-identities");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "Invalid",
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

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
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-identities");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "post-authentication-identity",
            MailAddress = (string?)null
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

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
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-identities");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "post-authentication-identity",
            MailAddress = string.Empty
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

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
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-identities");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "post-authentication-identity",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

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
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-identities");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "post-authentication-identity",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

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
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-identities");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAuthenticationIdentity = new
        {
            UniqueName = "post-authentication-identity",
            MailAddress = "foo-bar"
        };

        request.Content = JsonContent.Create(postAuthenticationIdentity);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postAuthenticationIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }
}