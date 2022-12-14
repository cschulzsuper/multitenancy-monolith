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
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
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
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, "invalid")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, "invalid")]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group1, "invalid")]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group2, "invalid")]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group1, "invalid")]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group2, "invalid")]
    public async Task SignIn_ShouldFail_WhenMemberDoesNotExist(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/groups/{group}/members/{member}/sign-in");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task SignIn_ShouldFail_WhenIdentityIsNotAssignedToMember(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/groups/{group}/members/{member}/sign-in");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, "invalid", TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, "invalid", TestConfiguration.Group2Chief)]
    [InlineData(TestConfiguration.DefaultIdentity, "invalid", TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.DefaultIdentity, "invalid", TestConfiguration.Group2Member)]
    [InlineData(TestConfiguration.GuestIdentity, "invalid", TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.GuestIdentity, "invalid", TestConfiguration.Group2Member)]
    public async Task SignIn_ShouldFail_WhenGroupDoesNotExist(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/groups/{group}/members/{member}/sign-in");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}