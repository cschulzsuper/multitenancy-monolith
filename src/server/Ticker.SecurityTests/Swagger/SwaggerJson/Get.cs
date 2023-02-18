using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
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
    [InlineData("v1-ticker")]
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
    [InlineData("v1", MockWebApplication.MockMember)]
    [InlineData("v1", MockWebApplication.MockTicker)]
    [InlineData("v1-ticker", MockWebApplication.MockMember)]
    [InlineData("v1-ticker", MockWebApplication.MockTicker)]
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
    [InlineData("v1", MockWebApplication.MockMember)]
    [InlineData("v1", MockWebApplication.MockTicker)]
    [InlineData("v1-ticker", MockWebApplication.MockMember)]
    [InlineData("v1-ticker", MockWebApplication.MockTicker)]
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