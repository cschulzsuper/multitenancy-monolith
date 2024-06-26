﻿using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Access.AccountRegistrationResource;

public sealed class Post 
{
    [Fact]
    public async Task Post_ShouldSucceed_WhenValid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("accountGroup", postAccountRegistration.AccountGroup), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("accountMember", postAccountRegistration.AccountMember), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("authenticationIdentity", postAccountRegistration.AuthenticationIdentity), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("mailAddress", postAccountRegistration.MailAddress), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("processState", AccountRegistrationProcessStates.New), (x.Key, (string?)x.Value)),
            x => Assert.Equal("snowflake", x.Key));

        using (var scope = application.CreateMultitenancyScope())
        {
            var createdMember = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.AuthenticationIdentity == postAccountRegistration.AuthenticationIdentity &&
                    x.AccountGroup == postAccountRegistration.AccountGroup &&
                    x.AccountMember == postAccountRegistration.AccountMember &&
                    x.MailAddress == postAccountRegistration.MailAddress);

            Assert.NotNull(createdMember);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAccountGroupExists()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = existingAccountRegistration.AccountGroup,
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.CreateMultitenancyScope())
        {
            var unchangedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountRegistration.Snowflake &&
                    x.AccountMember == existingAccountRegistration.AccountMember);

            Assert.NotNull(unchangedMember);
        }
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAccountGroupIsNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = (string?)null,
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAccountGroupIsEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = string.Empty,
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAccountGroupTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = new string(Enumerable.Repeat('a', 141).ToArray()),
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAccountGroupInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = "Invalid",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAccountMemberIsNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = (string?)null,
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAccountMemberIsEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = string.Empty,
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAccountMemberTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = new string(Enumerable.Repeat('a', 141).ToArray()),
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAccountMemberInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = "Invalid",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAuthenticationIdentityIsNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = (string?)null,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAuthenticationIdentityIsEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = string.Empty,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAuthenticationIdentityTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenAuthenticationIdentityInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = "Invalid",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = (string?)null
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = string.Empty
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressLocalPartTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenMailAddressInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var postAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "foo-bar"
        };

        request.Content = JsonContent.Create(postAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.CreateMultitenancyScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == postAccountRegistration.AccountGroup);

        Assert.Null(createdMember);
    }
}