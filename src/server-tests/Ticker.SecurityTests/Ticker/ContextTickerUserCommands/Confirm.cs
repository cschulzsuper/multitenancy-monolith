using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.ContextTickerUserCommands;

public sealed class Confirm : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public Confirm(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Theory]
    [InlineData(MockWebApplication.PendingMailAddress, MockWebApplication.PendingSecret)]
    public async Task Confirm_ShouldSucceed_WhenValid(string mail, string secret)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/_/confirm");

        var confirmRequest = new
        {
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = mail,
            Secret = secret,
            SecretToken = MockWebApplication.SecretTokens[mail]
        };

        request.Content = JsonContent.Create(confirmRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Confirm_ShouldBeForbidden_WhenGroupAbsent()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/_/confirm");

        var confirmRequest = new
        {
            AccountGroup = "absent",
            Mail = MockWebApplication.PendingMailAddress,
            Secret = MockWebApplication.PendingSecret,
            SecretToken = MockWebApplication.SecretTokens[MockWebApplication.PendingMailAddress]
        };

        request.Content = JsonContent.Create(confirmRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Confirm_ShouldBeForbidden_WhenTickerUserAbsent()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/_/confirm");

        var confirmRequest = new
        {
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = "absent@localhost.com",
            Secret = MockWebApplication.PendingSecret,
            SecretToken = MockWebApplication.SecretTokens[MockWebApplication.PendingMailAddress]
        };

        request.Content = JsonContent.Create(confirmRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Confirm_ShouldBeForbidden_WhenSecretIncorrect()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/_/confirm");

        var confirmRequest = new
        {
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = MockWebApplication.PendingMailAddress,
            Secret = "inavlid",
            SecretToken = MockWebApplication.SecretTokens[MockWebApplication.PendingMailAddress]
        };

        request.Content = JsonContent.Create(confirmRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Confirm_ShouldBeForbidden_WhenSecretTokenIncorrect()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/_/confirm");

        var confirmRequest = new
        {
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = MockWebApplication.PendingMailAddress,
            Secret = MockWebApplication.PendingSecret,
            SecretToken = Guid.NewGuid()
        };

        request.Content = JsonContent.Create(confirmRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}