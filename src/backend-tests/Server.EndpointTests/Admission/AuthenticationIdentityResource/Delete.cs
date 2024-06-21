using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Admission.AuthenticationIdentityResource;

public sealed class Delete 
{
    [Fact]
    public async Task Delete_ShouldSucceed_WhenExists()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/admission/authentication-identities/{existingAuthenticationIdentity.UniqueName}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.Services.CreateScope())
        {
            var deletedIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .GetQueryable()
                .SingleOrDefault(x => x.UniqueName == existingAuthenticationIdentity.UniqueName);

            Assert.Null(deletedIdentity);
        }
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenAbsent()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var absentAuthenticationIdentity = "absent-authentication-identity";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/admission/authentication-identities/{absentAuthenticationIdentity}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidAuthenticationIdentity = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/admission/authentication-identities/{invalidAuthenticationIdentity}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}