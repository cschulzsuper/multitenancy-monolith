using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.MemberSignIn;

public class SignIn : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public SignIn(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [InlineData(TestConfiguration.DefaultGroupAdminIdentity, TestConfiguration.DefaultGroup, TestConfiguration.DefaultGroupAdmin)]
    [InlineData(TestConfiguration.DefaultGroupGuestIdentity, TestConfiguration.DefaultGroup, TestConfiguration.DefaultGroupGuest)]
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
    [InlineData(TestConfiguration.DefaultGroupAdminIdentity, TestConfiguration.DefaultGroup, "invalid")]
    [InlineData(TestConfiguration.DefaultGroupGuestIdentity, TestConfiguration.DefaultGroup, "invalid")]
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
    [InlineData(TestConfiguration.DefaultGroupAdminIdentity, TestConfiguration.DefaultGroup, TestConfiguration.DefaultGroupGuest)]
    [InlineData(TestConfiguration.DefaultGroupGuestIdentity, TestConfiguration.DefaultGroup, TestConfiguration.DefaultGroupAdmin)]
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
    [InlineData(TestConfiguration.DefaultGroupAdminIdentity, "invalid", TestConfiguration.DefaultGroupGuest)]
    [InlineData(TestConfiguration.DefaultGroupGuestIdentity, "invalid", TestConfiguration.DefaultGroupAdmin)]
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