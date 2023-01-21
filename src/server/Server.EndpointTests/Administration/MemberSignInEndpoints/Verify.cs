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
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2chief)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    public async Task Verfiy_ShouldSucceed_WhenAuthorizationIsValid(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/members/me/verify");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2chief)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    public async Task Verfiy_ShouldBeUnauthorized_WhenAuthorizationIsInvalid(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/members/me/verify");
        request.Headers.Authorization = _factory.MockInvalidMemberAuthorizationHeader(identity, group, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}