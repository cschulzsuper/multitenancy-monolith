﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Authentication.IdentityResource;

public sealed class GetAll : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetAll(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Fact]
    [Trait("Category", "Endpoint.Security")]
    public async Task GetAll_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/authentication/identities");

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [Trait("Category", "Endpoint.Security")]
    [InlineData(TestConfiguration.ChiefIdentity)]
    [InlineData(TestConfiguration.DefaultIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task GetAll_ShouldBeForbidden_WhenNotAdmin(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task GetAll_ShouldSucceed(string identity)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/authentication/identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x => Assert.Equal(TestConfiguration.AdminIdentity, x.GetString("uniqueName")),
            x => Assert.Equal(TestConfiguration.ChiefIdentity, x.GetString("uniqueName")),
            x => Assert.Equal(TestConfiguration.GuestIdentity, x.GetString("uniqueName")),
            x => Assert.Equal(TestConfiguration.DefaultIdentity, x.GetString("uniqueName")));
    }
}