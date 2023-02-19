using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using Microsoft.Extensions.DependencyInjection;
using System;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using System.Linq;

namespace Ticker.TickerUserFlows;

public class ConfirmFlow : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ConfirmFlow(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Exceute()
    {
        await TickerUser_Create_ShouldSucceed();

        await TickerUser_Auth_ShouldSucceed_WithSecretStatePending();
        await TickerUser_Auth_ShouldBeForbidden();

        await TickerUser_Confirm_ShouldSucceedd_WithSecretStateConfirmed();
        await TickerUser_Confirm_ShouldBeForbidden();

        await TickerUser_Auth_ShouldSucceed_WithSecretStateConfirmed();
        await TickerUser_Auth_ShouldSucceed_WithSecretStateConfirmed();
    }

    private async Task TickerUser_Create_ShouldSucceed()
    {
        // TODO Replace with API call once endpoint is available

        var existingTickerUser = new TickerUser
        {
            Snowflake = 1,
            DisplayName = "Default",
            MailAddress = MockWebApplication.TicketUserMail,
            Secret = MockWebApplication.TicketUserSecret,
            SecretState = TickerUserSecretStates.Temporary,
            SecretToken = MockWebApplication.TicketUserSecretToken,
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            await scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .InsertAsync(existingTickerUser);
        }
    }

    private async Task TickerUser_Auth_ShouldSucceed_WithSecretStatePending()
    {
        // Arrange
        var authRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");
        var authContent = new
        {
            Client = MockWebApplication.Client,
            Group = MockWebApplication.Group,
            Mail = MockWebApplication.TicketUserMail,
            Secret = MockWebApplication.TicketUserSecret
        };

        authRequest.Content = JsonContent.Create(authContent);

        var client = _factory.CreateClient();

        // Act
        var authResponse = await client.SendAsync(authRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, authResponse.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var @object = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .Single();

            Assert.Equal(TickerUserSecretStates.Pending, @object.SecretState);
            Assert.NotEqual(MockWebApplication.TicketUserSecretToken, @object.SecretToken);
        }
    }

    private async Task TickerUser_Auth_ShouldBeForbidden()
    {
        // Arrange
        var authRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");
        var authContent = new
        {
            MockWebApplication.Client,
            MockWebApplication.Group,
            Mail = MockWebApplication.TicketUserMail,
            Secret = MockWebApplication.TicketUserSecret
        };

        authRequest.Content = JsonContent.Create(authContent);

        var client = _factory.CreateClient();

        // Act
        var authResponse = await client.SendAsync(authRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, authResponse.StatusCode);
    }

    private async Task TickerUser_Confirm_ShouldSucceedd_WithSecretStateConfirmed()
    {
        // Arrange
        Guid tickerUserSecretToken;

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var @object = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .Single();

            tickerUserSecretToken = @object.SecretToken;
        }

        var authRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/confirm");
        var authContent = new
        {
            MockWebApplication.Client,
            MockWebApplication.Group,
            Mail = MockWebApplication.TicketUserMail,
            Secret = MockWebApplication.TicketUserSecret,
            SecretToken = tickerUserSecretToken
        };

        authRequest.Content = JsonContent.Create(authContent);

        var client = _factory.CreateClient();

        // Act
        var authResponse = await client.SendAsync(authRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, authResponse.StatusCode);


        using (var scope = _factory.CreateMultitenancyScope())
        {
            var @object = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .Single();

            Assert.Equal(TickerUserSecretStates.Confirmed, @object.SecretState);
            Assert.NotEqual(Guid.Empty, @object.SecretToken);
        }
    }

    private async Task TickerUser_Confirm_ShouldBeForbidden()
    {
        // Arrange
        Guid tickerUserSecretToken;

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var @object = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .Single();

            tickerUserSecretToken = @object.SecretToken;
        }

        var authRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/confirm");
        var authContent = new
        {
            MockWebApplication.Client,
            MockWebApplication.Group,
            Mail = MockWebApplication.TicketUserMail,
            Secret = MockWebApplication.TicketUserSecret,
            SecretToken = tickerUserSecretToken
        };

        authRequest.Content = JsonContent.Create(authContent);

        var client = _factory.CreateClient();

        // Act
        var authResponse = await client.SendAsync(authRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, authResponse.StatusCode);
    }

    private async Task TickerUser_Auth_ShouldSucceed_WithSecretStateConfirmed()
    {
        // Arrange
        var authRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/me/auth");
        var authContent = new
        {
            MockWebApplication.Client,
            MockWebApplication.Group,
            Mail = MockWebApplication.TicketUserMail,
            Secret = MockWebApplication.TicketUserSecret
        };

        authRequest.Content = JsonContent.Create(authContent);

        var client = _factory.CreateClient();

        // Act
        var authResponse = await client.SendAsync(authRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, authResponse.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var @object = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .Single();

            Assert.Equal(TickerUserSecretStates.Confirmed, @object.SecretState);
            Assert.NotEqual(Guid.Empty, @object.SecretToken);
        }
    }
}