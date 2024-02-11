using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Admission.AuthenticationIdentityResource;

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Put_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var validAuthenticationIdentity = "valid-authentication-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{validAuthenticationIdentity}");
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
    public async Task Put_ShouldFail_WhenAuthorized(int mock)
    {
        // Arrange
        var validAuthenticationIdentity = "valid-authentication-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{validAuthenticationIdentity}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChief)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Put_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var validAuthenticationIdentity = "valid-authentication-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{validAuthenticationIdentity}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Fact]
    public async Task Post_ShouldBeUnauthorized_WhenInvalid()
    {
        // Arrange
        var validAuthenticationIdentity = "valid-authentication-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{validAuthenticationIdentity}");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader();
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}