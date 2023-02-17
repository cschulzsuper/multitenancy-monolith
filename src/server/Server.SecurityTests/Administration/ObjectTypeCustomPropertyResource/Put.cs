using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using ChristianSchulz.MultitenancyMonolith.Server;

namespace Administration.ObjectTypeCustomPropertyResource;

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
        var validObjectType = "valid-object-type";
        var validObjectTypeCustomProperty = "valid-object-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/object-types/{validObjectType}/custom-properties/{validObjectTypeCustomProperty}");

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "put-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

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
        var validObjectType = "valid-object-type";
        var validObjectTypeCustomProperty = "valid-object-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/object-types/{validObjectType}/custom-properties/{validObjectTypeCustomProperty}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);;

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "put-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}