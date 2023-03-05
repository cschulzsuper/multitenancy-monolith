using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
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
    [InlineData("v1")]
    [InlineData("v1-administration")]
    [InlineData("v1-authentication")]
    [InlineData("v1-authorization")]
    [InlineData("v1-business")]

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
    [InlineData("v1", MockWebApplication.MockAdmin)]
    [InlineData("v1", MockWebApplication.MockIdentity)]
    [InlineData("v1", MockWebApplication.MockDemo)]
    [InlineData("v1", MockWebApplication.MockChief)]
    [InlineData("v1", MockWebApplication.MockChiefObserver)]
    [InlineData("v1", MockWebApplication.MockMember)]
    [InlineData("v1", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-administration", MockWebApplication.MockAdmin)]
    [InlineData("v1-administration", MockWebApplication.MockIdentity)]
    [InlineData("v1-administration", MockWebApplication.MockDemo)]
    [InlineData("v1-administration", MockWebApplication.MockChief)]
    [InlineData("v1-administration", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-administration", MockWebApplication.MockMember)]
    [InlineData("v1-administration", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-authentication", MockWebApplication.MockAdmin)]
    [InlineData("v1-authentication", MockWebApplication.MockIdentity)]
    [InlineData("v1-authentication", MockWebApplication.MockDemo)]
    [InlineData("v1-authentication", MockWebApplication.MockChief)]
    [InlineData("v1-authentication", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-authentication", MockWebApplication.MockMember)]
    [InlineData("v1-authentication", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-authorization", MockWebApplication.MockAdmin)]
    [InlineData("v1-authorization", MockWebApplication.MockIdentity)]
    [InlineData("v1-authorization", MockWebApplication.MockDemo)]
    [InlineData("v1-authorization", MockWebApplication.MockChief)]
    [InlineData("v1-authorization", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-authorization", MockWebApplication.MockMember)]
    [InlineData("v1-authorization", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-business", MockWebApplication.MockAdmin)]
    [InlineData("v1-business", MockWebApplication.MockIdentity)]
    [InlineData("v1-business", MockWebApplication.MockDemo)]
    [InlineData("v1-business", MockWebApplication.MockChief)]
    [InlineData("v1-business", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-business", MockWebApplication.MockMember)]
    [InlineData("v1-business", MockWebApplication.MockMemberObserver)]
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
    [InlineData("v1", MockWebApplication.MockAdmin)]
    [InlineData("v1", MockWebApplication.MockIdentity)]
    [InlineData("v1", MockWebApplication.MockDemo)]
    [InlineData("v1", MockWebApplication.MockChief)]
    [InlineData("v1", MockWebApplication.MockChiefObserver)]
    [InlineData("v1", MockWebApplication.MockMember)]
    [InlineData("v1", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-administration", MockWebApplication.MockAdmin)]
    [InlineData("v1-administration", MockWebApplication.MockIdentity)]
    [InlineData("v1-administration", MockWebApplication.MockDemo)]
    [InlineData("v1-administration", MockWebApplication.MockChief)]
    [InlineData("v1-administration", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-administration", MockWebApplication.MockMember)]
    [InlineData("v1-administration", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-access", MockWebApplication.MockAdmin)]
    [InlineData("v1-access", MockWebApplication.MockIdentity)]
    [InlineData("v1-access", MockWebApplication.MockDemo)]
    [InlineData("v1-access", MockWebApplication.MockChief)]
    [InlineData("v1-access", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-access", MockWebApplication.MockMember)]
    [InlineData("v1-access", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-admission", MockWebApplication.MockAdmin)]
    [InlineData("v1-admission", MockWebApplication.MockIdentity)]
    [InlineData("v1-admission", MockWebApplication.MockDemo)]
    [InlineData("v1-admission", MockWebApplication.MockChief)]
    [InlineData("v1-admission", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-admission", MockWebApplication.MockMember)]
    [InlineData("v1-admission", MockWebApplication.MockMemberObserver)]
    [InlineData("v1-business", MockWebApplication.MockAdmin)]
    [InlineData("v1-business", MockWebApplication.MockIdentity)]
    [InlineData("v1-business", MockWebApplication.MockDemo)]
    [InlineData("v1-business", MockWebApplication.MockChief)]
    [InlineData("v1-business", MockWebApplication.MockChiefObserver)]
    [InlineData("v1-business", MockWebApplication.MockMember)]
    [InlineData("v1-business", MockWebApplication.MockMemberObserver)]
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