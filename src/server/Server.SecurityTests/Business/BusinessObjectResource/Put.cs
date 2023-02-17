using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Business.BusinessObjectResource;

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
        var validBusinessObject = "valid-business-object";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{validBusinessObject}");

        var putBusinessObject = new
        {
            UniqueName = "put-business-object",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(putBusinessObject);

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
        var validBusinessObject = "valid-business-object";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{validBusinessObject}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);;

        var putBusinessObject = new
        {
            UniqueName = "put-business-object",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}