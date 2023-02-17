using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Authorization.MemberCommands;

public sealed class Auth : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public Auth(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Auth_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/authorization/members/me/auth");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockIdentity, MockWebApplication.GroupChief)]
    [InlineData(MockWebApplication.MockIdentity, MockWebApplication.GroupMember)]
    [InlineData(MockWebApplication.MockDemo, MockWebApplication.GroupChief)]
    [InlineData(MockWebApplication.MockDemo, MockWebApplication.GroupMember)]
    public async Task Auth_ShouldSucceed_WhenValid(int mock, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/authorization/members/me/auth");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var authRequest = new
        {
            Client = MockWebApplication.Client,
            Group = MockWebApplication.Group,
            Member = member
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    public async Task Auth_ShouldFail_WhenMemberAbsent(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/authorization/members/me/auth");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var authRequest = new
        {
            Client = MockWebApplication.Client,
            Group = MockWebApplication.Group,
            Member = "absent"
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin, MockWebApplication.GroupChief)]
    [InlineData(MockWebApplication.MockAdmin, MockWebApplication.GroupMember)]
    public async Task Auth_ShouldFail_WhenIdentityUnassigned(int mock, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/authorization/members/me/auth");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var authRequest = new
        {
            Client = MockWebApplication.Client,
            Group = MockWebApplication.Group,
            Member = member
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin, MockWebApplication.GroupChief)]
    [InlineData(MockWebApplication.MockAdmin, MockWebApplication.GroupMember)]
    [InlineData(MockWebApplication.MockIdentity, MockWebApplication.GroupChief)]
    [InlineData(MockWebApplication.MockIdentity, MockWebApplication.GroupMember)]
    [InlineData(MockWebApplication.MockDemo, MockWebApplication.GroupChief)]
    [InlineData(MockWebApplication.MockDemo, MockWebApplication.GroupMember)]
    public async Task Auth_ShouldFail_WhenGroupAbsent(int mock, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/authorization/members/me/auth");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var authRequest = new
        {
            Client = MockWebApplication.Client,
            Group = "absent",
            Member = member
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin, MockWebApplication.GroupChief)]
    [InlineData(MockWebApplication.MockAdmin, MockWebApplication.GroupMember)]
    [InlineData(MockWebApplication.MockIdentity, MockWebApplication.GroupChief)]
    [InlineData(MockWebApplication.MockIdentity, MockWebApplication.GroupMember)]
    [InlineData(MockWebApplication.MockDemo, MockWebApplication.GroupChief)]
    [InlineData(MockWebApplication.MockDemo, MockWebApplication.GroupMember)]
    public async Task Auth_ShouldFail_WhenClientInvalid(int mock, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/authorization/members/me/auth");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var authRequest = new
        {
            Client = "invalid",
            Group = MockWebApplication.Group,
            Member = member
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}