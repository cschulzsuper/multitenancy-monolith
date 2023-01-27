using ChristianSchulz.MultitenancyMonolith.Aggregates.Authentication;
using ChristianSchulz.MultitenancyMonolith.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Authentication.IdentityResource;

public sealed class Get : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Get(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [Trait("Category", "Endpoint.Security")]
    [InlineData(TestConfiguration.ChiefIdentity)]
    [InlineData(TestConfiguration.DefaultIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Get_ShouldBeForbidden_WhenIdentityIsNotAdmin(string identity)
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName =  $"existing-identity-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/authentication/identities/{existingIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Get_ShouldSucceed_WhenIdentityExists(string identity)
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName =  $"existing-identity-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/authentication/identities/{existingIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal((x.Key, (string?)x.Value), ("mailAddress", existingIdentity.MailAddress)),
            x => Assert.Equal((x.Key, (string?)x.Value), ("uniqueName", existingIdentity.UniqueName)));
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Get_ShouldFail_WhenIdentityDoesNotExists(string identity)
    {
        // Arrange
        var absentIdentity = $"absent-identity-{Guid.NewGuid()}";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/authentication/identities/{absentIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Get_ShouldFail_WhenIdentityIsInvalid(string identity)
    {
        // Arrange
        var invalidIdentity = $"INVALID_IDENTITY_{Guid.NewGuid()}";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/authentication/identities/{invalidIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}