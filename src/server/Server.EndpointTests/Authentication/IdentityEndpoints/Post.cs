using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;
using Xunit;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Aggregates.Authentication;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Authentication.IdentityEndpoints;

public sealed class Post : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Post(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Post_ShouldReturnCreatedIdentity_WhenValidIdentityIsGiven(string identity)
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
        var content = await response.Content.ReadFromJsonAsync<JsonObject>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal((x.Key, (string?)x.Value), ("mailAddress", newIdentity.MailAddress)),
            x => Assert.Equal((x.Key, (string?)x.Value), ("uniqueName", newIdentity.UniqueName)));
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Post_ShouldCreateIdentityInRepository_WhenValidIdentityIsGiven(string identity)
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
        using var scope = _factory.Services.CreateScope();

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .GetQueryable()
            .SingleOrDefault(x =>
                x.UniqueName == newIdentity.UniqueName &&
                x.MailAddress == newIdentity.MailAddress &&
                x.Secret == newIdentity.Secret);

        Assert.NotNull(createdMember);
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Post_ShouldNotCreateIdentity_WhenIdentityUniqueNameIsEmpty(string identity)
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
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Post_ShouldNotCreateIdentity_WhenIdentityUniqueNameIsNull(string identity)
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
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Post_ShouldNotCreateIdentity_WhenIdentitySecretIsEmpty(string identity)
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
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Post_ShouldNotCreateIdentity_WhenIdentitySecretIsNull(string identity)
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
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Post_ShouldNotCreateIdentity_WhenIdentityMailAddressIsEmpty(string identity)
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
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Post_ShouldNotCreateIdentity_WhenIdentityMailAddressIsNull(string identity)
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
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Post_ShouldNotCreatIdentity_WhenIdentityMailAddressIsNotMailAddress(string identity)
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
    }
}
