using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Swagger.SwaggerJson;

public sealed class Get : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Get(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [Trait("Category", "Swagger.Security")]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.ChiefIdentity)]
    [InlineData(TestConfiguration.DefaultIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Get_ShouldBeForbidden_WhenClientIsEndpointTests(string identity)
    {
        // Arrange
        var productionFactory = _factory.WithWebHostBuilder(app => app
            .UseEnvironment("Production"));

        var request = new HttpRequestMessage(HttpMethod.Get, $"/swagger/v1/swagger.json");
        request.Headers.Authorization = productionFactory.MockValidIdentityAuthorizationHeader(identity);

        var client = productionFactory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [Trait("Category", "Swagger")]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.ChiefIdentity)]
    [InlineData(TestConfiguration.DefaultIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Get_ShouldSucceed_WhenClientIsSwagger(string identity)
    {
        // Arrange
        var productionFactory = _factory.WithWebHostBuilder(app => app
            .UseEnvironment("Production"));

        var request = new HttpRequestMessage(HttpMethod.Get, $"/swagger/v1/swagger.json");
        request.Headers.Authorization = productionFactory.MockValidIdentityAuthorizationHeader(
            claimName => claimName switch
            {
                "identity" => identity,
                "client" => "swagger",
                _ => throw new UnreachableException("Claim `{claimName}` is not supported.")
            });

        var client = productionFactory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
    }

    [Theory]
    [Trait("Category", "Swagger")]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.ChiefIdentity)]
    [InlineData(TestConfiguration.DefaultIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Get_ShouldSucceed_WhenEnvironmentIsDevelopment(string identity)
    {
        // Arrange
        var developmentFactory = _factory.WithInMemoryData();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/swagger/v1/swagger.json");
        request.Headers.Authorization = developmentFactory.MockValidIdentityAuthorizationHeader(
            claimName => claimName switch
            {
                "identity" => identity,
                "client" => "swagger",
                _ => throw new UnreachableException("Claim `{claimName}` is not supported.")
            });

        var client = developmentFactory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
    }
}