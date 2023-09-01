using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Schedule.PlannedJobResource;

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
        var validPlannedJob = "valid-planned-job";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/b1/schedule/planned-jobs/{validPlannedJob}");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    public async Task GetAll_ShouldFail_WhenAuthorized(int mock)
    {
        // Arrange
        var validPlannedJob = "valid-planned-job";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/b1/schedule/planned-jobs/{validPlannedJob}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChief)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    [InlineData(MockWebApplication.MockTicker)]
    public async Task Get_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var validPlannedJob = "valid-planned-job";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/b1/schedule/planned-jobs/{validPlannedJob}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Fact]
    public async Task Post_ShouldBeForbidden_WhenInvalid()
    {
        // Arrange
        var validPlannedJob = "valid-planned-job";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/b1/schedule/planned-jobs/{validPlannedJob}");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}