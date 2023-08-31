using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.TickerUserResource;

public sealed class Get : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Get(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Get_ShouldSucceed_WithoutCustomProperties()
    {
        // Arrange
        var existingTickerUser = new TickerUser
        {
            DisplayName = "Exiting Test User",
            MailAddress = $"{Guid.NewGuid()}@localhost",
            Secret = $"{Guid.NewGuid()}",
            SecretState = TickerUserSecretStates.Confirmed,
            SecretToken = Guid.NewGuid()
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(existingTickerUser);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/b1/ticker/ticker-users/{existingTickerUser.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("displayName", existingTickerUser.DisplayName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("mailAddress", existingTickerUser.MailAddress), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("secretState", existingTickerUser.SecretState), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("snowflake", existingTickerUser.Snowflake), (x.Key, (long?)x.Value)));
    }

    [Fact]
    public async Task Get_ShouldFail_WhenAbsent()
    {
        // Arrange
        var absentTickerUser = 1;

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/b1/ticker/ticker-users/{absentTickerUser}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Get_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidTickerUser = "invalid";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/b1/ticker/ticker-users/{invalidTickerUser}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}