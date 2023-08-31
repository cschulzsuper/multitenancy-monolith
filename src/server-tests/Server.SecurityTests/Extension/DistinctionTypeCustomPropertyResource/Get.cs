using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Extension.DistinctionTypeCustomPropertyResource;

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
        var validDistinctionType = "valid-distinction-type";
        var validDistinctionTypeCustomProperty = "valid-distinction-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/distinction-types/{validDistinctionType}/custom-properties/{validDistinctionTypeCustomProperty}");

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
    public async Task Get_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var validDistinctionType = "valid-distinction-type";
        var validDistinctionTypeCustomProperty = "valid-distinction-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/distinction-types/{validDistinctionType}/custom-properties/{validDistinctionTypeCustomProperty}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockChief)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Get_ShouldSucceed_WhenAuthorized(int mock)
    {
        // Arrange
        var validDistinctionType = "valid-distinction-type";
        var validDistinctionTypeCustomProperty = "valid-distinction-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/distinction-types/{validDistinctionType}/custom-properties/{validDistinctionTypeCustomProperty}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}