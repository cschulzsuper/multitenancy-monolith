using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Access.AccountGroupResource;

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
        var validAccountGroup = "valid-account-group";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-groups/{validAccountGroup}");

        var putAccountGroup = new
        {
            UniqueName = "put-account-group"
        };

        request.Content = JsonContent.Create(putAccountGroup);

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
        var validAccountGroup = "valid-account-group";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/access/account-groups/{validAccountGroup}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var putAccountGroup = new
        {
            UniqueName = "put-account-group"
        };

        request.Content = JsonContent.Create(putAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}