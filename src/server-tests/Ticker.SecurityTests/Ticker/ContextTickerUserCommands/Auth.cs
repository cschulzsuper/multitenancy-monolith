using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ticker.ContextTickerUserCommands;

public sealed class Auth : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public Auth(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory.Mock(output);
    }

    [Theory]
    [InlineData(MockWebApplication.ConfirmedMailAddress, MockWebApplication.ConfirmedSecret)]
    [InlineData(MockWebApplication.ResetMailAddress, MockWebApplication.ResetSecret)]
    public async Task Auth_ShouldSucceed_WhenValid(string mail, string secret)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");

        var authRequest = new
        {
            ClientName = MockWebApplication.ClientName,
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = mail,
            Secret = secret
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(MockWebApplication.PendingMailAddress, MockWebApplication.PendingSecret)]
    [InlineData(MockWebApplication.InvalidMailAddress, MockWebApplication.InvalidSecret)]
    public async Task Auth_ShouldBeForbidden_WhenSecretStateInvalid(string mail, string secret)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");

        var authRequest = new
        {
            ClientName = MockWebApplication.ClientName,
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = mail,
            Secret = secret
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Auth_ShouldBeForbidden_WhenClientAbsent()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");

        var authRequest = new
        {
            ClientName = "absent",
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = MockWebApplication.ConfirmedMailAddress,
            Secret = MockWebApplication.ConfirmedSecret
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        var x = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Auth_ShouldBeForbidden_WhenGroupAbsent()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");

        var authRequest = new
        {
            ClientName = MockWebApplication.ClientName,
            AccountGroup = "absent",
            Mail = MockWebApplication.ConfirmedMailAddress,
            Secret = MockWebApplication.ConfirmedSecret
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Auth_ShouldBeForbidden_WhenTickerUserAbsent()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");

        var authRequest = new
        {
            ClientName = MockWebApplication.ClientName,
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = "absent@localhost.com",
            Secret = MockWebApplication.ConfirmedSecret
        };

        request.Content = JsonContent.Create(authRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Auth_ShouldBeForbidden_WhenSecretIncorrect()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");

        var authRequest = new
        {
            ClientName = MockWebApplication.ClientName,
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = MockWebApplication.ConfirmedMailAddress,
            Secret = "inavlid"
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