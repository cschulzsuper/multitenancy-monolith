using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using ChristianSchulz.MultitenancyMonolith.Aggregates.Authentication;
using ChristianSchulz.MultitenancyMonolith.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Authentication.IdentityEndpoints;

public sealed class Post : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Post(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [Trait("Category", "Endpoint.Security")]
    [InlineData(TestConfiguration.ChiefIdentity)]
    [InlineData(TestConfiguration.DefaultIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Delete_ShouldBeForbidden_WhenIdentityIsNotAdmin(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var newIdentity = new
        {
            UniqueName = $"new-identity-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(newIdentity);

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
    public async Task Post_ShouldSucceed_WhenValidIdentityIsGiven(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var newIdentity = new
        {
            UniqueName = $"new-identity-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(newIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal((x.Key, (string?)x.Value), ("mailAddress", newIdentity.MailAddress)),
            x => Assert.Equal((x.Key, (string?)x.Value), ("uniqueName", newIdentity.UniqueName)));

        using (var scope = _factory.Services.CreateScope())
        {
            var createdMember = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.UniqueName == newIdentity.UniqueName &&
                    x.MailAddress == newIdentity.MailAddress &&
                    x.Secret == newIdentity.Secret);

            Assert.NotNull(createdMember);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Post_ShouldFail_WhenIdentityUniqueNameIsEmpty(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var newIdentity = new
        {
            UniqueName = string.Empty,
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(newIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == newIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Post_ShouldFail_WhenIdentityUniqueNameIsNull(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var newIdentity = new
        {
            UniqueName = (string?)null,
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(newIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == newIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Post_ShouldFail_WhenIdentitySecretIsEmpty(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var newIdentity = new
        {
            UniqueName = $"new-identity-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = string.Empty
        };

        request.Content = JsonContent.Create(newIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == newIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Post_ShouldFail_WhenIdentitySecretIsNull(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var newIdentity = new
        {
            UniqueName = $"new-identity-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = (string?)null,
        };

        request.Content = JsonContent.Create(newIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == newIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Post_ShouldFail_WhenIdentityMailAddressIsEmpty(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var newIdentity = new
        {
            UniqueName = $"new-identity-{Guid.NewGuid()}",
            MailAddress = string.Empty,
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(newIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == newIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Post_ShouldFail_WhenIdentityMailAddressIsNull(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var newIdentity = new
        {
            UniqueName = $"new-identity-{Guid.NewGuid()}",
            MailAddress = (string?)null,
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(newIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == newIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Post_ShouldFail_WhenIdentityMailAddressIsNotMailAddress(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var newIdentity = new
        {
            UniqueName = $"new-identity-{Guid.NewGuid()}",
            MailAddress = "foo-bar",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(newIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateScope();

        var createdIdentity = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == newIdentity.UniqueName);

        Assert.Null(createdIdentity);
    }
}