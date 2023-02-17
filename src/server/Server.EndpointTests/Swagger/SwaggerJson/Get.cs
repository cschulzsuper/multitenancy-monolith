﻿using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.OpenApi.Readers;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Swagger.SwaggerJson;

public sealed class Get : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Get(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Theory]
    [InlineData("v1-server")]
    [InlineData("v1-server-administration")]
    [InlineData("v1-server-authentication")]
    [InlineData("v1-server-authorization")]
    [InlineData("v1-server-business")]
    public async Task Get_ShouldSucceed_WhenValid(string doc)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/swagger/{doc}/swagger.json");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStreamAsync();

        new OpenApiStreamReader().Read(content, out OpenApiDiagnostic diagnostic);

        Assert.Empty(diagnostic.Errors);
    }
}