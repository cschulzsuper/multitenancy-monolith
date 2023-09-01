using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Admission.AuthenticationIdentityResource;

public sealed class Head : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Head(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Head_ShouldFail_WhenNotAuthenticated()
    {
        // Arrange
        var validAuthenticationIdentity = "valid-authentication-identity";

        var request = new HttpRequestMessage(HttpMethod.Head, $"/api/a1/admission/authentication-identities/{validAuthenticationIdentity}");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChief)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Head_ShouldFail_WhenAuthorized(int mock)
    {
        // Arrange
        var validAuthenticationIdentity = "valid-authentication-identity";

        var request = new HttpRequestMessage(HttpMethod.Head, $"/api/a1/admission/authentication-identities/{validAuthenticationIdentity}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentLength);
    }

    [Fact]
    public async Task Head_ShouldFail_WhenInvalid()
    {
        // Arrange
        var validAuthenticationIdentity = "valid-authentication-identity";

        var request = new HttpRequestMessage(HttpMethod.Head, $"/api/a1/admission/authentication-identities/{validAuthenticationIdentity}");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Null(response.Content.Headers.ContentLength);
    }
}