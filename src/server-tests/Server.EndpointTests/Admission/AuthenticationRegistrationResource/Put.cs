using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;
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

namespace Admission.AuthenticationRegistrationResource;

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"put-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var changedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAuthenticationRegistration.Snowflake &&
                    x.AuthenticationIdentity == putAuthenticationRegistration.AuthenticationIdentity &&
                    x.MailAddress == putAuthenticationRegistration.MailAddress);

            Assert.NotNull(changedMember);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidAuthenticationRegistration = "invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-registrations/{invalidAuthenticationRegistration}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"put-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationRegistration);

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
        var absentAuthenticationRegistration = 1;

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-registrations/{absentAuthenticationRegistration}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"put-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAuthenticationIdentityExists()
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

        var additionalAuthenticationRegistration = new AuthenticationRegistration
        {
            AuthenticationIdentity = $"additional-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AuthenticationRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            Secret = $"{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .Insert(existingAuthenticationRegistration, additionalAuthenticationRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationRegistration = new
        {
            additionalAuthenticationRegistration.AuthenticationIdentity,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

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
    public async Task Put_ShouldFail_WhenAuthenticationIdentityNull()
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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationRegistration = new
        {
            AccountGroup = $"put-authentication-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"put-authentication-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = (string?)null,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationRegistration);

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationRegistration = new
        {
            AuthenticationIdentity = string.Empty,
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationRegistration);

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationRegistration = new
        {
            AuthenticationIdentity = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationRegistration);

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationRegistration = new
        {
            AuthenticationIdentity = "Invalid",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationRegistration);

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"post-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = (string?)null
        };

        request.Content = JsonContent.Create(putAuthenticationRegistration);

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"post-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = string.Empty
        };

        request.Content = JsonContent.Create(putAuthenticationRegistration);

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"post-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(putAuthenticationRegistration);

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"post-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(putAuthenticationRegistration);

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

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"post-authentication-registration-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}