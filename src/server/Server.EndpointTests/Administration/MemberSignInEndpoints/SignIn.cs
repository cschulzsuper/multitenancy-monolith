using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.MemberSignInEndpoints;

public sealed class SignIn : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public SignIn(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Guest)]
    public async Task SignIn_ShouldSucceed_WhenMemberIsValid(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/groups/{group}/members/{member}/sign-in");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, "invalid")]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, "invalid")]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, "invalid")]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, "invalid")]
    public async Task SignIn_ShouldFail_WhenMemberDoesNotExist(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/groups/{group}/members/{member}/sign-in");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Guest)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Admin)]
    public async Task SignIn_ShouldFail_WhenIdentityIsNotAssignedToMember(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/groups/{group}/members/{member}/sign-in");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, "invalid", TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.GuestIdentity, "invalid", TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.AdminIdentity, "invalid", TestConfiguration.DefaultGroup2Guest)]
    [InlineData(TestConfiguration.GuestIdentity, "invalid", TestConfiguration.DefaultGroup2Admin)]
    public async Task SignIn_ShouldFail_WhenGroupDoesNotExist(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/groups/{group}/members/{member}/sign-in");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }
}