using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Business.BusinessObjectResource;

public sealed class Post : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Post(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Post_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/business/business-objects");
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockChief)]
    public async Task Post_ShouldFail_WhenAuthorized(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/business/business-objects");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Post_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/business/business-objects");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Fact]
    public async Task Post_ShouldBeUnauthorized_WhenInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/a1/business/business-objects");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader();
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}