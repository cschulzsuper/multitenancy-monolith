using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Extension.DistinctionTypeCustomPropertyResource;

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
        var validDistinctionType = "required-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/extension/distinction-types/{validDistinctionType}/custom-properties");

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = "post-distinction-type-custom-property"
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

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
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Post_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var validDistinctionType = "required-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/extension/distinction-types/{validDistinctionType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = "post-distinction-type-custom-property"
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}