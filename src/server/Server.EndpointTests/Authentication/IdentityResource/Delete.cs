using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Authentication.IdentityResource;

public sealed class Delete : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Delete(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Delete_ShouldSucceed_WhenExists()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/authentication/identities/{existingIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var deletedIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x => x.UniqueName == existingIdentity.UniqueName);

            Assert.Null(deletedIdentity);
        }
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenAbsent()
    {
        // Arrange
        var absentIdentity = "absent-identity";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/authentication/identities/{absentIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var client = _factory.CreateClient();

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
        var invalidIdentity = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/authentication/identities/{invalidIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}