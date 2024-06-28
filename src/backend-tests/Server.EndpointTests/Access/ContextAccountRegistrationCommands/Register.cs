using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Access.ContextAccountRegistrationCommands;

public sealed class Register 
{
    [Fact]
    public async Task Register_ShouldSucceed_WhenValid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = $"register-account-registration-member-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = application.Services.CreateScope();
        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x =>
                x.AccountGroup == registerAccountRegistration.AccountGroup &&
                x.AccountMember == registerAccountRegistration.AccountMember &&
                x.AuthenticationIdentity == MockWebApplication.AuthenticationIdentity &&
                x.ProcessState == AccountRegistrationProcessStates.New &&
                x.MailAddress == registerAccountRegistration.MailAddress);

        Assert.NotNull(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenAccountGroupExists()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountRegistration = new AccountRegistration
        {
            AuthenticationIdentity = $"existing-account-registration-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = $"register-account-registration-member-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid()
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            existingAccountRegistration.AccountGroup,
            existingAccountRegistration.AccountMember,
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.Services.CreateScope())
        {
            var unchangedRegistration = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountRegistration.Snowflake &&
                    x.AuthenticationIdentity == existingAccountRegistration.AuthenticationIdentity &&
                    x.MailAddress == existingAccountRegistration.MailAddress &&
                    x.AccountGroup == existingAccountRegistration.AccountGroup &&
                    x.AccountMember == existingAccountRegistration.AccountMember);

            Assert.NotNull(unchangedRegistration);
        }
    }

    [Fact]
    public async Task Register_ShouldFail_WhenAccountGroupNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            AccountGroup = (string?)null,
            AccountMember = "account-member",
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == registerAccountRegistration.AccountGroup);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenAccountGroupEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            AccountGroup = string.Empty,
            AccountMember = "account-member",
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == registerAccountRegistration.AccountGroup);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenAccountGroupTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            AccountGroup = new string(Enumerable.Repeat('a', 141).ToArray()),
            AccountMember = "account-member",
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == registerAccountRegistration.AccountGroup);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenAccountGroupInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            AccountGroup = "Invalid",
            AccountMember = "account-member",
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountGroup == registerAccountRegistration.AccountGroup);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenAccountMemberNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = (string?)null,
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountMember == registerAccountRegistration.AccountMember);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenAccountMemberEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = string.Empty,
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountMember == registerAccountRegistration.AccountMember);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenAccountMemberTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "info@localhost",
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.AccountMember == registerAccountRegistration.AccountMember);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenMailAddressNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = "account-member",
            MailAddress = (string?)null
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.MailAddress == registerAccountRegistration.MailAddress);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenMailAddressEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = "account-member",
            MailAddress = string.Empty
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenMailAddressTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = "account-member",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.MailAddress == registerAccountRegistration.MailAddress);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenMailAddressLocalPartTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = "account-member",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.MailAddress == registerAccountRegistration.MailAddress);

        Assert.Null(createdRegistration);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenMailAddressInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/access/account-registrations/_/register");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var registerAccountRegistration = new
        {
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = "account-member",
            MailAddress = "foo-bar"
        };

        request.Content = JsonContent.Create(registerAccountRegistration);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = application.Services.CreateScope();

        var createdRegistration = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountRegistration>>()
            .GetQueryable()
            .SingleOrDefault(x => x.MailAddress == registerAccountRegistration.MailAddress);

        Assert.Null(createdRegistration);
    }
}