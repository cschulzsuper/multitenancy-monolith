using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.Member;

public sealed class Get : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Get(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Guest)]
    public async Task Get_ShouldSucceed_WhenAuthorizationIsValid(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/members/{member}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1Admin, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1Guest, TestConfiguration.DefaultGroup1Admin)]
    public async Task Get_ShouldReturnDefaultGroup1Member_WhenAuthorizationForDefaultGroup1(string identity, string member, string otherMember)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/members/{otherMember}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, TestConfiguration.DefaultGroup1, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();

        // Assert
        Assert.NotNull(content);
        Assert.Equal(otherMember, content.RootElement.GetProperty("uniqueName").GetString());
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2Admin, TestConfiguration.DefaultGroup2Guest)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2Guest, TestConfiguration.DefaultGroup2Admin)]
    public async Task Get_ShouldReturnDefaultGroup2Member_WhenAuthorizationForDefaultGroup2(string identity, string member, string otherMember)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/members/{otherMember}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, TestConfiguration.DefaultGroup2, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();

        // Assert
        Assert.NotNull(content);
        Assert.Equal(otherMember, content.RootElement.GetProperty("uniqueName").GetString());
    }
}
