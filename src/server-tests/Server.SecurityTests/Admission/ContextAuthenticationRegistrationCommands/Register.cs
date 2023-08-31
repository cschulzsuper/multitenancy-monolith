﻿using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Admission.ContextAuthenticationRegistrationCommands;

public sealed class Register : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Register(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Register_ShouldFail_WhenNotAuthenticated()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-registrations/_/register");
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChief)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Register_ShouldFail_WhenAuthorized(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-registrations/_/register");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock); ;
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChief)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Register_ShouldFail_WhenInvalid(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/admission/authentication-registrations/_/register");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader(mock);
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}