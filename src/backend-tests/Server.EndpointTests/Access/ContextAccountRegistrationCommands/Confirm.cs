using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Access.ContextAccountRegistrationCommands;

public sealed class Confirm 
{
    [Fact]
    public async Task Confirm_ShouldSucceed_WhenProcessStateNew()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountRegistration = new AccountRegistration
        {
            AuthenticationIdentity = MockWebApplication.AuthenticationIdentity,
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

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-registrations/_/confirm");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var confirmRequest = new
        {
            existingAccountRegistration.AccountGroup,
            existingAccountRegistration.ProcessToken
        };

        request.Content = JsonContent.Create(confirmRequest);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Confirm_ShouldFail_WhenProcessStateConfirmed()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountRegistration = new AccountRegistration
        {
            AuthenticationIdentity = MockWebApplication.AuthenticationIdentity,
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

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-registrations/_/confirm");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var confirmRequest = new
        {
            existingAccountRegistration.AccountGroup,
            existingAccountRegistration.ProcessToken
        };

        request.Content = JsonContent.Create(confirmRequest);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Confirm_ShouldFail_WhenProcessStateApproved()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountRegistration = new AccountRegistration
        {
            AuthenticationIdentity = MockWebApplication.AuthenticationIdentity,
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

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-registrations/_/confirm");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var confirmRequest = new
        {
            existingAccountRegistration.AccountGroup,
            existingAccountRegistration.ProcessToken
        };

        request.Content = JsonContent.Create(confirmRequest);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Confirm_ShouldFail_WhenProcessStateCompleted()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAccountRegistration = new AccountRegistration
        {
            AuthenticationIdentity = MockWebApplication.AuthenticationIdentity,
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

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-registrations/_/confirm");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var confirmRequest = new
        {
            existingAccountRegistration.AccountGroup,
            existingAccountRegistration.ProcessToken
        };

        request.Content = JsonContent.Create(confirmRequest);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}