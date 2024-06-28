using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Extension.ObjectTypeResource;

public sealed class Get 
{
    [Fact]
    public async Task Get_ShouldSucceed_WhenBusinessObject()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var objectTypeBusinessObject = "business-object";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/object-types/{objectTypeBusinessObject}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("area", "business"), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("collection", "business-objects"), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("displayName", "Business Object"), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", "business-object"), (x.Key, (string?)x.Value)));
    }

    [Fact]
    public async Task Get_ShouldFail_WhenAbsent()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var absentObjectType = "absent-object-type";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/object-types/{absentObjectType}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Get_ShouldFail_WhenInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidObjectType = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/object-types/{invalidObjectType}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}