using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Access.ContextAccountMemberCommands;

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
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-members/_/auth");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockIdentity, MockWebApplication.AccountGroupChief)]
    [InlineData(MockWebApplication.MockIdentity, MockWebApplication.AccountGroupMember)]
    [InlineData(MockWebApplication.MockDemo, MockWebApplication.AccountGroupChief)]
    [InlineData(MockWebApplication.MockDemo, MockWebApplication.AccountGroupMember)]
    public async Task Auth_ShouldSucceed_WhenValid(int mock, string accountMember)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-members/_/auth");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var authRequest = new
        {
            ClientName = MockWebApplication.ClientName,
            MockWebApplication.AccountGroup,
            AccountMember = accountMember
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
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-members/_/auth");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var authRequest = new
        {
            ClientName = MockWebApplication.ClientName,
            MockWebApplication.AccountGroup,
            AccountMember = "absent"
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
    [InlineData(MockWebApplication.MockAdmin, MockWebApplication.AccountGroupChief)]
    [InlineData(MockWebApplication.MockAdmin, MockWebApplication.AccountGroupMember)]
    public async Task Auth_ShouldFail_WhenIdentityUnassigned(int mock, string accountMember)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-members/_/auth");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var authRequest = new
        {
            ClientName = MockWebApplication.ClientName,
            MockWebApplication.AccountGroup,
            AccountMember = accountMember
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
    [InlineData(MockWebApplication.MockAdmin, MockWebApplication.AccountGroupChief)]
    [InlineData(MockWebApplication.MockAdmin, MockWebApplication.AccountGroupMember)]
    [InlineData(MockWebApplication.MockIdentity, MockWebApplication.AccountGroupChief)]
    [InlineData(MockWebApplication.MockIdentity, MockWebApplication.AccountGroupMember)]
    [InlineData(MockWebApplication.MockDemo, MockWebApplication.AccountGroupChief)]
    [InlineData(MockWebApplication.MockDemo, MockWebApplication.AccountGroupMember)]
    public async Task Auth_ShouldFail_WhenGroupAbsent(int mock, string accountMember)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-members/_/auth");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var authRequest = new
        {
            ClientName = MockWebApplication.ClientName,
            AccountGroup = "absent",
            AccountMember = accountMember
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
    [InlineData(MockWebApplication.MockAdmin, MockWebApplication.AccountGroupChief)]
    [InlineData(MockWebApplication.MockAdmin, MockWebApplication.AccountGroupMember)]
    [InlineData(MockWebApplication.MockIdentity, MockWebApplication.AccountGroupChief)]
    [InlineData(MockWebApplication.MockIdentity, MockWebApplication.AccountGroupMember)]
    [InlineData(MockWebApplication.MockDemo, MockWebApplication.AccountGroupChief)]
    [InlineData(MockWebApplication.MockDemo, MockWebApplication.AccountGroupMember)]
    public async Task Auth_ShouldFail_WhenClientInvalid(int mock, string accountMember)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-members/_/auth");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var authRequest = new
        {
            ClientName = "invalid",
            MockWebApplication.AccountGroup,
            AccountMember = accountMember
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