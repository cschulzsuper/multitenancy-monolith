using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Access.AccountGroupResource;

public sealed class Get : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Get(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Get_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var validAccountGroup = "valid-account-group";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/access/account-groups/{validAccountGroup}");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    public async Task Get_ShouldFail_WhenAuthorized(int mock)
    {
        // Arrange
        var validAccountGroup = "valid-account-group";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/access/account-groups/{validAccountGroup}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChief)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Get_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var validAccountGroup = "valid-account-group";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/access/account-groups/{validAccountGroup}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Fact]
    public async Task Get_ShouldBeUnauthorized_WhenInvalid()
    {
        // Arrange
        var validAccountGroup = "valid-account-group";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/access/account-groups/{validAccountGroup}");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}