using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Aggregates.Authentication;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.IdentityEndpoints;

public sealed class Delete : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Delete(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Delete_ShouldSucceed_WhenExistingIdentityIsGiven(string identity)
    {
        // Arrange
        var existingIdentity = $"existing-identity-{Guid.NewGuid()}";

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(new Identity
                {
                    Snowflake = 1,
                    UniqueName = existingIdentity,
                    MailAddress = "info@localhost",
                    Secret = "foo-bar"
                });
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/identities/{existingIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

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
                .SingleOrDefault(x => x.UniqueName == existingIdentity);

            Assert.Null(deletedIdentity);
        }
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Delete_ShouldFail_WhenIdentityDoesNotExist(string identity)
    {
        // Arrange
        var absentIdentity = $"absent-identity-{Guid.NewGuid()}";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/identities/{absentIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Delete_ShouldFail_WhenIdentityUniqueNameIsInvalid(string identity)
    {
        // Arrange
        var invalidIdentity = $"INVALID_IDENTITY_{Guid.NewGuid()}";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/identities/{invalidIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
