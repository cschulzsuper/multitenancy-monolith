﻿using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Access.AccountRegistrationResource;

public sealed class GetAll : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetAll(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task GetAll_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/access/account-registrations");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    public async Task Post_ShouldSucceed_WhenAuthorized(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/access/account-registrations");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChief)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task GetAll_ShouldBeForbidden_WhenNotAuthorized(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/access/account-registrations");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}