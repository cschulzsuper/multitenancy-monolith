using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Admission.AuthenticationRegistrationCommands;

public sealed class Approve : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Approve(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Approve_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var validAuthenticationRegistration = 1;

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/admission/authentication-registrations/{validAuthenticationRegistration}/approve");
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    public async Task Register_ShouldFail_WhenAuthorized(int mock)
    {
        // Arrange
        var validAuthenticationRegistration = 1;

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/admission/authentication-registrations/{validAuthenticationRegistration}/approve");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChief)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Register_ShouldBeUnauthorized_WhenInvalid(int mock)
    {
        // Arrange
        var validAuthenticationRegistration = 1;

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/admission/authentication-registrations/{validAuthenticationRegistration}/approve");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}