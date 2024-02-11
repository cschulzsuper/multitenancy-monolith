using ChristianSchulz.MultitenancyMonolith.Backend.Server;
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
    [InlineData("a1")]
    [InlineData("a1-extension")]
    [InlineData("a1-authentication")]
    [InlineData("a1-authorization")]
    [InlineData("a1-business")]
    [InlineData("a1-schedule")]

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
    [InlineData("a1", MockWebApplication.MockAdmin)]
    [InlineData("a1", MockWebApplication.MockIdentity)]
    [InlineData("a1", MockWebApplication.MockDemo)]
    [InlineData("a1", MockWebApplication.MockChief)]
    [InlineData("a1", MockWebApplication.MockChiefObserver)]
    [InlineData("a1", MockWebApplication.MockMember)]
    [InlineData("a1", MockWebApplication.MockMemberObserver)]
    [InlineData("a1-extension", MockWebApplication.MockAdmin)]
    [InlineData("a1-extension", MockWebApplication.MockIdentity)]
    [InlineData("a1-extension", MockWebApplication.MockDemo)]
    [InlineData("a1-extension", MockWebApplication.MockChief)]
    [InlineData("a1-extension", MockWebApplication.MockChiefObserver)]
    [InlineData("a1-extension", MockWebApplication.MockMember)]
    [InlineData("a1-extension", MockWebApplication.MockMemberObserver)]
    [InlineData("a1-authentication", MockWebApplication.MockAdmin)]
    [InlineData("a1-authentication", MockWebApplication.MockIdentity)]
    [InlineData("a1-authentication", MockWebApplication.MockDemo)]
    [InlineData("a1-authentication", MockWebApplication.MockChief)]
    [InlineData("a1-authentication", MockWebApplication.MockChiefObserver)]
    [InlineData("a1-authentication", MockWebApplication.MockMember)]
    [InlineData("a1-authentication", MockWebApplication.MockMemberObserver)]
    [InlineData("a1-authorization", MockWebApplication.MockAdmin)]
    [InlineData("a1-authorization", MockWebApplication.MockIdentity)]
    [InlineData("a1-authorization", MockWebApplication.MockDemo)]
    [InlineData("a1-authorization", MockWebApplication.MockChief)]
    [InlineData("a1-authorization", MockWebApplication.MockChiefObserver)]
    [InlineData("a1-authorization", MockWebApplication.MockMember)]
    [InlineData("a1-authorization", MockWebApplication.MockMemberObserver)]
    [InlineData("a1-business", MockWebApplication.MockAdmin)]
    [InlineData("a1-business", MockWebApplication.MockIdentity)]
    [InlineData("a1-business", MockWebApplication.MockDemo)]
    [InlineData("a1-business", MockWebApplication.MockChief)]
    [InlineData("a1-business", MockWebApplication.MockChiefObserver)]
    [InlineData("a1-business", MockWebApplication.MockMember)]
    [InlineData("a1-business", MockWebApplication.MockMemberObserver)]
    [InlineData("a1-schedule", MockWebApplication.MockAdmin)]
    [InlineData("a1-schedule", MockWebApplication.MockIdentity)]
    [InlineData("a1-schedule", MockWebApplication.MockDemo)]
    [InlineData("a1-schedule", MockWebApplication.MockChief)]
    [InlineData("a1-schedule", MockWebApplication.MockChiefObserver)]
    [InlineData("a1-schedule", MockWebApplication.MockMember)]
    [InlineData("a1-schedule", MockWebApplication.MockMemberObserver)]
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
    [InlineData("a1", MockWebApplication.MockAdmin)]
    [InlineData("a1", MockWebApplication.MockIdentity)]
    [InlineData("a1", MockWebApplication.MockDemo)]
    [InlineData("a1", MockWebApplication.MockChief)]
    [InlineData("a1", MockWebApplication.MockChiefObserver)]
    [InlineData("a1", MockWebApplication.MockMember)]
    [InlineData("a1", MockWebApplication.MockMemberObserver)]
    [InlineData("a1-extension", MockWebApplication.MockAdmin)]
    [InlineData("a1-extension", MockWebApplication.MockIdentity)]
    [InlineData("a1-extension", MockWebApplication.MockDemo)]
    [InlineData("a1-extension", MockWebApplication.MockChief)]
    [InlineData("a1-extension", MockWebApplication.MockChiefObserver)]
    [InlineData("a1-extension", MockWebApplication.MockMember)]
    [InlineData("a1-extension", MockWebApplication.MockMemberObserver)]
    [InlineData("a1-access", MockWebApplication.MockAdmin)]
    [InlineData("a1-access", MockWebApplication.MockIdentity)]
    [InlineData("a1-access", MockWebApplication.MockDemo)]
    [InlineData("a1-access", MockWebApplication.MockChief)]
    [InlineData("a1-access", MockWebApplication.MockChiefObserver)]
    [InlineData("a1-access", MockWebApplication.MockMember)]
    [InlineData("a1-access", MockWebApplication.MockMemberObserver)]
    [InlineData("a1-admission", MockWebApplication.MockAdmin)]
    [InlineData("a1-admission", MockWebApplication.MockIdentity)]
    [InlineData("a1-admission", MockWebApplication.MockDemo)]
    [InlineData("a1-admission", MockWebApplication.MockChief)]
    [InlineData("a1-admission", MockWebApplication.MockChiefObserver)]
    [InlineData("a1-admission", MockWebApplication.MockMember)]
    [InlineData("a1-admission", MockWebApplication.MockMemberObserver)]
    [InlineData("a1-business", MockWebApplication.MockAdmin)]
    [InlineData("a1-business", MockWebApplication.MockIdentity)]
    [InlineData("a1-business", MockWebApplication.MockDemo)]
    [InlineData("a1-business", MockWebApplication.MockChief)]
    [InlineData("a1-business", MockWebApplication.MockChiefObserver)]
    [InlineData("a1-business", MockWebApplication.MockMember)]
    [InlineData("a1-business", MockWebApplication.MockMemberObserver)]
    [InlineData("a1-schedule", MockWebApplication.MockAdmin)]
    [InlineData("a1-schedule", MockWebApplication.MockIdentity)]
    [InlineData("a1-schedule", MockWebApplication.MockDemo)]
    [InlineData("a1-schedule", MockWebApplication.MockChief)]
    [InlineData("a1-schedule", MockWebApplication.MockChiefObserver)]
    [InlineData("a1-schedule", MockWebApplication.MockMember)]
    [InlineData("a1-schedule", MockWebApplication.MockMemberObserver)]
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