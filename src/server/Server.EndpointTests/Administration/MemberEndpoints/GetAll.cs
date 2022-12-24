using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.MemberEndpoints;

public sealed class GetAll : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetAll(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1Guest)]
    public async Task GetAll_ShouldReturnDefaultGroup1Members(string identity, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, TestConfiguration.DefaultGroup1, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x => Assert.Equal(TestConfiguration.DefaultGroup1Admin, x.GetString("uniqueName")),
            x => Assert.Equal(TestConfiguration.DefaultGroup1Guest, x.GetString("uniqueName")));
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2Guest)]
    public async Task GetAll_ShouldReturnDefaultGroup2Members(string identity, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, TestConfiguration.DefaultGroup2, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x => Assert.Equal(TestConfiguration.DefaultGroup2Admin, x.GetString("uniqueName")),
            x => Assert.Equal(TestConfiguration.DefaultGroup2Guest, x.GetString("uniqueName")));
    }
}
