using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.MemberSignInEndpoints;

public sealed class IdentitySignInTests : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public IdentitySignInTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Guest)]
    public async Task Verfiy_ShouldSucceed_WhenAuthorizationIsValid(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/members/me/verify");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Guest)]
    public async Task Verfiy_ShouldFail_WhenAuthorizationIsInvalid(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/members/me/verify");
        request.Headers.Authorization = _factory.MockInvalidMemberAuthorizationHeader(identity, group, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}