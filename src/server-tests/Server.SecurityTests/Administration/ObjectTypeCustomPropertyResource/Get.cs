using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using ChristianSchulz.MultitenancyMonolith.Server;

namespace Administration.ObjectTypeCustomPropertyResource;

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
        var validObjectType = "valid-object-type";
        var validObjectTypeCustomProperty = "valid-object-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/administration/object-types/{validObjectType}/custom-properties/{validObjectTypeCustomProperty}");

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
        var validObjectType = "valid-object-type";
        var validObjectTypeCustomProperty = "valid-object-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/administration/object-types/{validObjectType}/custom-properties/{validObjectTypeCustomProperty}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);;

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}