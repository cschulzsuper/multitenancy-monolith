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
using System.Threading.Tasks;
using Xunit;

namespace Admission.AuthenticationRegistrationCommands;

public sealed class Approve 
{
    [Fact]
    public async Task Approve_ShouldSucceed_WhenProcessStateConfirmed()
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

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}/approve");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.Services.CreateScope())
        {
            var approvedRegistration = scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.ProcessState == AuthenticationRegistrationProcessStates.Approved);

            Assert.NotNull(approvedRegistration);
        }
    }

    [Fact]
    public async Task Approve_ShouldFail_WhenProcessStateNew()
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

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}/approve");
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
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAuthenticationRegistration.Snowflake &&
                    x.ProcessState == AuthenticationRegistrationProcessStates.Approved);

            Assert.Null(unchangedRegistration);
        }
    }

    [Fact]
    public async Task Approve_ShouldFail_WhenProcessStateApproved()
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

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}/approve");
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
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAuthenticationRegistration.Snowflake &&
                    x.ProcessState == AuthenticationRegistrationProcessStates.Approved);

            Assert.NotNull(unchangedRegistration);
        }
    }

    [Fact]
    public async Task Approve_ShouldFail_WhenProcessStateCompleted()
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

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-registrations/{existingAuthenticationRegistration.Snowflake}/approve");
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
                .GetRequiredService<IRepository<AuthenticationRegistration>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAuthenticationRegistration.Snowflake &&
                    x.ProcessState == AuthenticationRegistrationProcessStates.Approved);

            Assert.Null(unchangedRegistration);
        }
    }
}