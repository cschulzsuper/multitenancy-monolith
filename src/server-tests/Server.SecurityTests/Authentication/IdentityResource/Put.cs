using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Authentication.IdentityResource;

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Put_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");

        var putIdentity = new
        {
            UniqueName = "put-identity",
            MailAddress = "put-info@localhost",
            Secret = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Put_ShouldBeForbidden_WhenNotAdmin(int mock)
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);;

        var putIdentity = new
        {
            UniqueName = "put-identity",
            MailAddress = "put-info@localhost",
            Secret = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}