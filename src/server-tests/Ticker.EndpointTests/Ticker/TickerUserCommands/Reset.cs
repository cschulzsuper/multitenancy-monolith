using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Ticker.TickerUserCommands;

public sealed class Reset : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Reset(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory.Mock(output);
    }

    [Fact]
    public async Task Reset_ShouldSucceed_WhenValid()
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


        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/{existingTickerUser.Snowflake}/reset");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var resetTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(resetTickerUser);
            Assert.Equal(TickerUserSecretStates.Reset, resetTickerUser.SecretState);
        }
    }

    [Fact]
    public async Task Reset_ShouldFail_WhenAbsent()
    {
        // Arrange
        var absentTickerUser = 1;

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/{absentTickerUser}/reset");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Reset_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidTickerUser = "invalid";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/{invalidTickerUser}/reset");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}