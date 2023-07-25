using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;

namespace Schedule.PlannedJobResource;

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
        var validPlannedJob = "valid-planned-job";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/schedule/planned-jobs/{validPlannedJob}");

        var putPlannedJob = new
        {
            Expression = "*/1 * * * *"
        };

        request.Content = JsonContent.Create(putPlannedJob);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockTicker)]
    public async Task Put_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var validPlannedJob = "valid-planned-job";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/schedule/planned-jobs/{validPlannedJob}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var putPlannedJob = new
        {
            Expression = "*/1 * * * *"
        };

        request.Content = JsonContent.Create(putPlannedJob);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}