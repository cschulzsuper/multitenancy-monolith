using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Swagger.SwaggerJson;

public sealed class Get : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Get(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Theory]
    [InlineData("v1-server")]
    [InlineData("v1-server-administration")]
    [InlineData("v1-server-authentication")]
    [InlineData("v1-server-authorization")]
    [InlineData("v1-server-business")]

    public async Task Get_ShouldBeUnauthorized_WhenNotAuthenticated(string doc)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/swagger/{doc}/swagger.json");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData("v1-server", MockWebApplication.MockAdmin)]
    [InlineData("v1-server", MockWebApplication.MockIdentity)]
    [InlineData("v1-server", MockWebApplication.MockDemo)]
    [InlineData("v1-server", MockWebApplication.MockChief)]
    [InlineData("v1-server", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-server", MockWebApplication.MockMember)]
    [InlineData("v1-server", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-server-administration", MockWebApplication.MockAdmin)]
    [InlineData("v1-server-administration", MockWebApplication.MockIdentity)]
    [InlineData("v1-server-administration", MockWebApplication.MockDemo)]
    [InlineData("v1-server-administration", MockWebApplication.MockChief)]
    [InlineData("v1-server-administration", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-server-administration", MockWebApplication.MockMember)]
    [InlineData("v1-server-administration", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-server-authentication", MockWebApplication.MockAdmin)]
    [InlineData("v1-server-authentication", MockWebApplication.MockIdentity)]
    [InlineData("v1-server-authentication", MockWebApplication.MockDemo)]
    [InlineData("v1-server-authentication", MockWebApplication.MockChief)]
    [InlineData("v1-server-authentication", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-server-authentication", MockWebApplication.MockMember)]
    [InlineData("v1-server-authentication", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-server-authorization", MockWebApplication.MockAdmin)]
    [InlineData("v1-server-authorization", MockWebApplication.MockIdentity)]
    [InlineData("v1-server-authorization", MockWebApplication.MockDemo)]
    [InlineData("v1-server-authorization", MockWebApplication.MockChief)]
    [InlineData("v1-server-authorization", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-server-authorization", MockWebApplication.MockMember)]
    [InlineData("v1-server-authorization", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-server-business", MockWebApplication.MockAdmin)]
    [InlineData("v1-server-business", MockWebApplication.MockIdentity)]
    [InlineData("v1-server-business", MockWebApplication.MockDemo)]
    [InlineData("v1-server-business", MockWebApplication.MockChief)]
    [InlineData("v1-server-business", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-server-business", MockWebApplication.MockMember)]
    [InlineData("v1-server-business", MockWebApplication.MockMemberObserver)]
    public async Task Get_ShouldBeForbidden_WhenInvalidClient(string doc, int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/swagger/{doc}/swagger.json");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData("v1-server", MockWebApplication.MockAdmin)]
    [InlineData("v1-server", MockWebApplication.MockIdentity)]
    [InlineData("v1-server", MockWebApplication.MockDemo)]
    [InlineData("v1-server", MockWebApplication.MockChief)]
    [InlineData("v1-server", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-server", MockWebApplication.MockMember)]
    [InlineData("v1-server", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-server-administration", MockWebApplication.MockAdmin)]
    [InlineData("v1-server-administration", MockWebApplication.MockIdentity)]
    [InlineData("v1-server-administration", MockWebApplication.MockDemo)]
    [InlineData("v1-server-administration", MockWebApplication.MockChief)]
    [InlineData("v1-server-administration", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-server-administration", MockWebApplication.MockMember)]
    [InlineData("v1-server-administration", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-server-authentication", MockWebApplication.MockAdmin)]
    [InlineData("v1-server-authentication", MockWebApplication.MockIdentity)]
    [InlineData("v1-server-authentication", MockWebApplication.MockDemo)]
    [InlineData("v1-server-authentication", MockWebApplication.MockChief)]
    [InlineData("v1-server-authentication", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-server-authentication", MockWebApplication.MockMember)]
    [InlineData("v1-server-authentication", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-server-authorization", MockWebApplication.MockAdmin)]
    [InlineData("v1-server-authorization", MockWebApplication.MockIdentity)]
    [InlineData("v1-server-authorization", MockWebApplication.MockDemo)]
    [InlineData("v1-server-authorization", MockWebApplication.MockChief)]
    [InlineData("v1-server-authorization", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-server-authorization", MockWebApplication.MockMember)]
    [InlineData("v1-server-authorization", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-server-business", MockWebApplication.MockAdmin)]
    [InlineData("v1-server-business", MockWebApplication.MockIdentity)]
    [InlineData("v1-server-business", MockWebApplication.MockDemo)]
    [InlineData("v1-server-business", MockWebApplication.MockChief)]
    [InlineData("v1-server-business", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-server-business", MockWebApplication.MockMember)]
    [InlineData("v1-server-business", MockWebApplication.MockMemberObserver)]
    public async Task Get_ShouldSucceed_WhenValidClient(string doc, int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/swagger/{doc}/swagger.json");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock, "swagger");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}