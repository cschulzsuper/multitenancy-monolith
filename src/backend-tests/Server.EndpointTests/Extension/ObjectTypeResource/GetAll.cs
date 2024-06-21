using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Extension.ObjectTypeResource;

public sealed class GetAll 
{
    [Fact]
    public async Task GetAll_ShouldSucceed()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/extension/object-types");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x =>
            {
                Assert.Equal("business-object", x.GetString("uniqueName"));
                Assert.Equal("Business Object", x.GetString("displayName"));
                Assert.Equal("business", x.GetString("area"));
                Assert.Equal("business-objects", x.GetString("collection"));
            });
    }
}