using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Access.AccountRegistrationCommands;

public sealed class Approve 
{
    [Fact]
    public async Task Approve_ShouldSucceed_WhenProcessStateConfirmed()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountRegistration = new AccountRegistration
        {
            AuthenticationIdentity = $"existing-account-registration-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = $"register-account-registration-member-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.Confirmed,
            ProcessToken = Guid.NewGuid()
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-registrations/{existingAccountRegistration.Snowflake}/approve");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.Services.CreateScope())
        {
            var approvedRegistration = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountRegistration.Snowflake &&
                    x.ProcessState == AccountRegistrationProcessStates.Approved);

            Assert.NotNull(approvedRegistration);
        }
    }

    [Fact]
    public async Task Approve_ShouldFail_WhenProcessStateNew()
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

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-registrations/{existingAccountRegistration.Snowflake}/approve");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.Services.CreateScope())
        {
            var unchangedRegistration = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountRegistration.Snowflake &&
                    x.ProcessState == AccountRegistrationProcessStates.Approved);

            Assert.Null(unchangedRegistration);
        }
    }

    [Fact]
    public async Task Approve_ShouldFail_WhenProcessStateApproved()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountRegistration = new AccountRegistration
        {
            AuthenticationIdentity = $"existing-account-registration-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = $"register-account-registration-member-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.Approved,
            ProcessToken = Guid.NewGuid()
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-registrations/{existingAccountRegistration.Snowflake}/approve");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.Services.CreateScope())
        {
            var unchangedRegistration = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountRegistration.Snowflake &&
                    x.ProcessState == AccountRegistrationProcessStates.Approved);

            Assert.NotNull(unchangedRegistration);
        }
    }

    [Fact]
    public async Task Approve_ShouldFail_WhenProcessStateCompleted()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountRegistration = new AccountRegistration
        {
            AuthenticationIdentity = $"existing-account-registration-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = $"register-account-registration-member-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.Completed,
            ProcessToken = Guid.NewGuid()
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-registrations/{existingAccountRegistration.Snowflake}/approve");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = application.Services.CreateScope())
        {
            var unchangedRegistration = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountRegistration.Snowflake &&
                    x.ProcessState == AccountRegistrationProcessStates.Approved);

            Assert.Null(unchangedRegistration);
        }
    }
}