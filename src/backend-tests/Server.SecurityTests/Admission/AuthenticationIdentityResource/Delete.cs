﻿using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Admission.AuthenticationIdentityResource;

public sealed class Delete : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Delete(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Delete_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var validAuthenticationIdentity = "valid-authentication-identity";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/admission/authentication-identities/{validAuthenticationIdentity}");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    public async Task Delete_ShouldFail_WhenAuthorized(int mock)
    {
        // Arrange
        var validAuthenticationIdentity = "valid-authentication-identity";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/admission/authentication-identities/{validAuthenticationIdentity}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChief)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Delete_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var validAuthenticationIdentity = "valid-authentication-identity";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/admission/authentication-identities/{validAuthenticationIdentity}");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Fact]
    public async Task Delete_ShouldBeUnauthorized_WhenInvalid()
    {
        // Arrange
        var validAuthenticationIdentity = "valid-authentication-identity";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/admission/authentication-identities/{validAuthenticationIdentity}");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}