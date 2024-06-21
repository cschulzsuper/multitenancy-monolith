using Microsoft.OpenApi.Readers;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Swagger.SwaggerJson;

public sealed class Get 
{
    [Theory]
    [InlineData("a1")]
    [InlineData("a1-extension")]
    [InlineData("a1-access")]
    [InlineData("a1-admission")]
    [InlineData("a1-business")]
    [InlineData("a1-schedule")]
    public async Task Get_ShouldSucceed_WhenValid(string doc)
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/openapi/{doc}.json");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStreamAsync();

        new OpenApiStreamReader().Read(content, out OpenApiDiagnostic diagnostic);

        Assert.Empty(diagnostic.Errors);
    }
}