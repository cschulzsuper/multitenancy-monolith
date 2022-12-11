using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;
using Xunit;
using System.Text.Json.Nodes;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.MemberEndpoints;

public sealed class Get : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Get(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Admin, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Guest, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Admin, TestConfiguration.DefaultGroup2Guest)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Guest, TestConfiguration.DefaultGroup2Admin)]
    public async Task Get_ShouldReturnMember_WhenMemberExists(string authIdentity, string authGroup, string authMember, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/members/{member}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(authIdentity, authGroup, authMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        var content = await response.Content.ReadFromJsonAsync<JsonObject>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Collection(content,
            x => Assert.Equal((x.Key, (string?)x.Value), ("uniqueName", member)));
    }
}
