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

namespace Admission.ContextAuthenticationRegistrationCommands;

public sealed class Register : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Register(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Register_ShouldSucceed_WhenValid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"register-authentication-registration-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "secret"
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x =>
                x.AuthenticationIdentity == registerAuthenticationRegistration.AuthenticationIdentity &&
                x.ProcessState == AuthenticationRegistrationProcessStates.New &&
                x.MailAddress == registerAuthenticationRegistration.MailAddress);

        Assert.NotNull(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenAuthenticationIdentityExists()
    {
        // Arrange
        var existingAuthenticationRegistration = new AuthenticationRegistration
        {
            AuthenticationIdentity = $"existing-authentication-registration-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar",
            ProcessState = AuthenticationRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid()
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .Insert(existingAuthenticationRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            existingAuthenticationRegistration.AuthenticationIdentity,
            MailAddress = "info@localhost",
            Secret = "secret"
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateScope())
        {
            var unchangedRegistration = scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAuthenticationRegistration.Snowflake &&
                    x.AuthenticationIdentity == existingAuthenticationRegistration.AuthenticationIdentity &&
                    x.MailAddress == existingAuthenticationRegistration.MailAddress &&
                    x.Secret == existingAuthenticationRegistration.Secret);

            Assert.NotNull(unchangedRegistration);
        }
    }

    [Fact]
    public async Task Register_ShouldFail_WhenAuthenticationIdentityNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            AuthenticationIdentity = (string?)null,
            MailAddress = "info@localhost",
            Secret = "secret"
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenAuthenticationIdentityEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            AuthenticationIdentity = string.Empty,
            MailAddress = "info@localhost",
            Secret = "secret"
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AuthenticationIdentity == string.Empty);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenAuthenticationIdentityTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            AuthenticationIdentity = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "info@localhost",
            Secret = "secret"
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenAuthenticationIdentityInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            AuthenticationIdentity = "Invalid",
            MailAddress = "info@localhost",
            Secret = "secret"
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AuthenticationIdentity == registerAuthenticationRegistration.AuthenticationIdentity);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenMailAddressNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            AuthenticationIdentity = "register-authentication-registration",
            MailAddress = (string?)null,
            Secret = "secret"
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.MailAddress == registerAuthenticationRegistration.MailAddress);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenMailAddressEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            AuthenticationIdentity = "register-authentication-registration",
            MailAddress = string.Empty,
            Secret = "secret"
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.MailAddress == registerAuthenticationRegistration.MailAddress);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenMailAddressTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            AuthenticationIdentity = "register-authentication-registration",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}",
            Secret = "secret"
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.MailAddress == registerAuthenticationRegistration.MailAddress);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenMailAddressLocalPartTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            AuthenticationIdentity = "register-authentication-registration",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}",
            Secret = "secret"
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.MailAddress == registerAuthenticationRegistration.MailAddress);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenMailAddressInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            AuthenticationIdentity = "register-authentication-registration",
            MailAddress = "foo-bar",
            Secret = "secret"
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.MailAddress == registerAuthenticationRegistration.MailAddress);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenSecretNull()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"register-authentication-registration-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = (string?)null
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.Secret == registerAuthenticationRegistration.Secret);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenSecretEmpty()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"register-authentication-registration-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = string.Empty
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.Secret == registerAuthenticationRegistration.Secret);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenSecretTooLong()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/admission/authentication-registrations/_/register");

        var registerAuthenticationRegistration = new
        {
            AuthenticationIdentity = $"register-authentication-registration-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = new string(Enumerable.Repeat('a', 141).ToArray())
        };

        request.Content = JsonContent.Create(registerAuthenticationRegistration);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.Secret == registerAuthenticationRegistration.Secret);

        Assert.Null(createdRegistration);
    }
}