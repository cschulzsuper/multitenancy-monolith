using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System;
using Microsoft.Extensions.DependencyInjection;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System.Linq;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;

namespace Ticker.TickerUserResource;

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Put_ShouldRespectMultitenancy()
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

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(existingTickerUser);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-users/{existingTickerUser.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(MockWebApplication.Group1);

        var putTickerUser = new
        {
            DisplayName = "Put Test User",
            MailAddress = $"{Guid.NewGuid()}@localhost",
        };

        request.Content = JsonContent.Create(putTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            var updatedTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerUser);
            Assert.Equal(existingTickerUser.DisplayName, updatedTickerUser.DisplayName);
            Assert.Equal(existingTickerUser.MailAddress, updatedTickerUser.MailAddress);
            Assert.Equal(existingTickerUser.Secret, updatedTickerUser.Secret);
            Assert.Equal(existingTickerUser.SecretState, updatedTickerUser.SecretState);
            Assert.Equal(existingTickerUser.SecretToken, updatedTickerUser.SecretToken);
        }
    }
}