using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Access.AccountRegistrationResource;

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Put_ShouldSucceed_WhenValid()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"put-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"put-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"put-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var changedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountRegistration.Snowflake &&
                    x.AccountGroup == putAccountRegistration.AccountGroup &&
                    x.AccountMember == putAccountRegistration.AccountMember &&
                    x.AuthenticationIdentity == putAccountRegistration.AuthenticationIdentity &&
                    x.MailAddress == putAccountRegistration.MailAddress);

            Assert.NotNull(changedMember);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidAccountRegistration = "invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{invalidAccountRegistration}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"put-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"put-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"put-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAbsent()
    {
        // Arrange
        var absentAccountRegistration = 1;

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{absentAccountRegistration}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"put-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"put-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"put-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAccountGroupExists()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        var additionalAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"additional-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"additional-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"additional-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "additional@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration, additionalAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = additionalAccountRegistration.AccountGroup,
            AccountMember = $"put-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"put-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var unchangedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountRegistration.Snowflake &&
                    x.AccountGroup == existingAccountRegistration.AccountGroup);

            Assert.NotNull(unchangedMember);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAccountGroupNull()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = (string?)null,
            AccountMember = $"put-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"put-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAccountGroupEmpty()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = string.Empty,
            AccountMember = $"put-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"put-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAccountGroupTooLong()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = new string(Enumerable.Repeat('a', 141).ToArray()),
            AccountMember = $"put-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"put-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAccountGroupInvalid()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = "Invalid",
            AccountMember = $"put-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"put-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAccountMemberNull()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"put-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = (string?)null,
            AuthenticationIdentity = $"put-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAccountMemberEmpty()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"put-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = string.Empty,
            AuthenticationIdentity = $"put-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAccountMemberTooLong()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"put-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = new string(Enumerable.Repeat('a', 141).ToArray()),
            AuthenticationIdentity = $"put-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAccountMemberInvalid()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"put-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = "Invalid",
            AuthenticationIdentity = $"put-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAuthenticationIdentityNull()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"put-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"put-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = (string?)null,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAuthenticationIdentityEmpty()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"put-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"put-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = string.Empty,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAuthenticationIdentityTooLong()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"put-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"put-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAuthenticationIdentityInvalid()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"put-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"put-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = "Invalid",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressNull()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = (string?)null
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressEmpty()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = string.Empty
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressTooLong()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressLocalPartTooLong()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressInvalid()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountRegistration = new
        {
            AccountGroup = $"post-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"post-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"post-account-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putAccountRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}