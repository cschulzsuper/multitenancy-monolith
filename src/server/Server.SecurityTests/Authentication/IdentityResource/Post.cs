﻿using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Authentication.IdentityResource;

public sealed class Post : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Post(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Post_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");

        var postIdentity = new
        {
            UniqueName = "post-identity",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Post_ShouldBeForbidden_WhenNotAdmin(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);;

        var postIdentity = new
        {
            UniqueName = "post-identity",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(postIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}