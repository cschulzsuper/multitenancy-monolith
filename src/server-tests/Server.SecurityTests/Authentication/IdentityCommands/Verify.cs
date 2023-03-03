using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Authentication.IdentityCommands;

public sealed class Verify : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Verify(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Verify_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities/me/verify");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    public async Task Verify_ShouldSucceed_WhenValid(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities/me/verify");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);;

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(MockWebApplication.MockChief)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Verify_ShouldBeUnauthorized_WhenNotIdentity(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities/me/verify");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChief)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Verify_ShouldBeUnauthorized_WhenInvalid(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities/me/verify");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}