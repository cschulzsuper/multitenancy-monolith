using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Admission.ContextAuthenticationRegistrationCommands;

public sealed class Confirm 
{
    [Fact]
    public async Task Confirm_ShouldSucceed_WhenProcessStateNew()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAuthenticationRegistration = new AuthenticationRegistration
        {
            AuthenticationIdentity = $"existing-authentication-registration-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar",
            ProcessState = AuthenticationRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid()
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .Insert(existingAuthenticationRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-registrations/_/confirm");

        var confirmRequest = new
        {
            existingAuthenticationRegistration.AuthenticationIdentity,
            existingAuthenticationRegistration.Secret,
            existingAuthenticationRegistration.ProcessToken
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

        var existingAuthenticationRegistration = new AuthenticationRegistration
        {
            AuthenticationIdentity = $"existing-authentication-registration-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar",
            ProcessState = AuthenticationRegistrationProcessStates.Confirmed,
            ProcessToken = Guid.NewGuid()
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .Insert(existingAuthenticationRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-registrations/_/confirm");

        var confirmRequest = new
        {
            existingAuthenticationRegistration.AuthenticationIdentity,
            existingAuthenticationRegistration.Secret,
            existingAuthenticationRegistration.ProcessToken
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

        var existingAuthenticationRegistration = new AuthenticationRegistration
        {
            AuthenticationIdentity = $"existing-authentication-registration-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar",
            ProcessState = AuthenticationRegistrationProcessStates.Approved,
            ProcessToken = Guid.NewGuid()
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .Insert(existingAuthenticationRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-registrations/_/confirm");

        var confirmRequest = new
        {
            existingAuthenticationRegistration.AuthenticationIdentity,
            existingAuthenticationRegistration.Secret,
            existingAuthenticationRegistration.ProcessToken
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

        var existingAuthenticationRegistration = new AuthenticationRegistration
        {
            AuthenticationIdentity = $"existing-authentication-registration-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar",
            ProcessState = AuthenticationRegistrationProcessStates.Completed,
            ProcessToken = Guid.NewGuid()
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .Insert(existingAuthenticationRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-registrations/_/confirm");

        var confirmRequest = new
        {
            existingAuthenticationRegistration.AuthenticationIdentity,
            existingAuthenticationRegistration.Secret,
            existingAuthenticationRegistration.ProcessToken
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