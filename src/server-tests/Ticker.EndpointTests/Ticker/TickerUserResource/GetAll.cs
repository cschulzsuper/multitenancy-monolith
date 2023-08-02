using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.TickerUserResource;

public sealed class GetAll : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetAll(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task GetAll_ShouldSucceed_WhenValid()
    {
        // Arrange
        var existingTickerUser1 = new TickerUser
        {
            DisplayName = "Exiting Test User 1",
            MailAddress = $"{Guid.NewGuid()}@localhost",
            Secret = $"{Guid.NewGuid()}",
            SecretState = TickerUserSecretStates.Confirmed,
            SecretToken = Guid.NewGuid()
        };

        var existingTickerUser2 = new TickerUser
        {
            DisplayName = "Exiting Test User 2",
            MailAddress = $"{Guid.NewGuid()}@localhost",
            Secret = $"{Guid.NewGuid()}",
            SecretState = TickerUserSecretStates.Confirmed,
            SecretToken = Guid.NewGuid()
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(existingTickerUser1, existingTickerUser2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/ticker/ticker-users");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("displayName")),
            x =>
            {
                Assert.Equal(existingTickerUser1.Snowflake, x.GetProperty("snowflake").GetInt64());
                Assert.Equal(existingTickerUser1.DisplayName, x.GetString("displayName"));
                Assert.Equal(existingTickerUser1.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingTickerUser1.SecretState, x.GetString("secretState"));
            },
            x =>
            {
                Assert.Equal(existingTickerUser2.Snowflake, x.GetProperty("snowflake").GetInt64());
                Assert.Equal(existingTickerUser2.DisplayName, x.GetString("displayName"));
                Assert.Equal(existingTickerUser2.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingTickerUser2.SecretState, x.GetString("secretState"));
            });
    }
}