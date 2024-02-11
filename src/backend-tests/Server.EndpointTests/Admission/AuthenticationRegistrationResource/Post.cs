using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;
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

namespace Admission.AuthenticationRegistrationResource;

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
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationRegistration = new
        {
            AccountGroup = $"post-authentication-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-authentication-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("authenticationIdentity", postAuthenticationRegistration.AuthenticationIdentity), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("mailAddress", postAuthenticationRegistration.MailAddress), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("processState", AuthenticationRegistrationProcessStates.New), (x.Key, (string?)x.Value)),
            x => Assert.Equal("snowflake", x.Key));

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x =>
                x.AuthenticationIdentity == postAuthenticationRegistration.AuthenticationIdentity &&
                x.MailAddress == postAuthenticationRegistration.MailAddress);

        Assert.NotNull(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAuthenticationIdentityExists()
    {
        // Arrange
        var existingAuthenticationRegistration = new AuthenticationRegistration
        {
            AuthenticationIdentity = $"existing-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AuthenticationRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            Secret = $"{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .Insert(existingAuthenticationRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationRegistration = new
        {
            existingAuthenticationRegistration.AuthenticationIdentity,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var unchangedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAuthenticationRegistration.Snowflake &&
                    x.AuthenticationIdentity == existingAuthenticationRegistration.AuthenticationIdentity);

            Assert.NotNull(unchangedMember);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAuthenticationIdentityIsNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationRegistration = new
        {
            AuthenticationIdentity = (string?)null,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AuthenticationIdentity == postAuthenticationRegistration.AuthenticationIdentity);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAuthenticationIdentityIsEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationRegistration = new
        {
            AuthenticationIdentity = string.Empty,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AuthenticationIdentity == postAuthenticationRegistration.AuthenticationIdentity);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAuthenticationIdentityTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationRegistration = new
        {
            AuthenticationIdentity = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AuthenticationIdentity == postAuthenticationRegistration.AuthenticationIdentity);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAuthenticationIdentityInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationRegistration = new
        {
            AuthenticationIdentity = "Invalid",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AuthenticationIdentity == postAuthenticationRegistration.AuthenticationIdentity);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"post-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = (string?)null
        };

        request.Content = JsonContent.Create(postAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AuthenticationIdentity == postAuthenticationRegistration.AuthenticationIdentity);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"post-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = string.Empty
        };

        request.Content = JsonContent.Create(postAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AuthenticationIdentity == postAuthenticationRegistration.AuthenticationIdentity);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"post-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(postAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AuthenticationIdentity == postAuthenticationRegistration.AuthenticationIdentity);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressLocalPartTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"post-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(postAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AuthenticationIdentity == postAuthenticationRegistration.AuthenticationIdentity);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var postAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"post-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "foo-bar"
        };

        request.Content = JsonContent.Create(postAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AuthenticationIdentity == postAuthenticationRegistration.AuthenticationIdentity);

        Assert.Null(createdMember);
    }
}