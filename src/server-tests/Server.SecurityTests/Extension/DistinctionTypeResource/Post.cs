﻿using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using ChristianSchulz.MultitenancyMonolith.Server;

namespace Extension.DistinctionTypeResource;

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
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/extension/distinction-types");

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Post_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/extension/distinction-types");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var postDistinctionType = new
        {
            UniqueName = "post-distinction-type",
            ObjectType = "business-object",
            DisplayName = "Post Distinction Type"
        };

        request.Content = JsonContent.Create(postDistinctionType);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}