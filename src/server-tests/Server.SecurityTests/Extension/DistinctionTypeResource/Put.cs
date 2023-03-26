using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using ChristianSchulz.MultitenancyMonolith.Server;

namespace Extension.DistinctionTypeResource;

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
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/extension/distinction-types/{validDistinctionType}");

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
    public async Task Put_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var validDistinctionType = "valid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/extension/distinction-types/{validDistinctionType}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var putDistinctionType = new
        {
            UniqueName = "put-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}