using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Ticker.ContextTickerUserCommands;

public sealed class Confirm : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _factory;

    public Confirm(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Confirm_ShouldRespectMultitenancy()
    {
        // Arrange
        var existingTickerUser = new TickerUser
        {
            DisplayName = "Default",
            MailAddress = MockWebApplication.Mail,
            Secret = "default",
            SecretState = TickerUserSecretStates.Pending,
            SecretToken = Guid.NewGuid()
        };

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.AccountGroup2))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(existingTickerUser);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/b1/ticker/ticker-users/_/confirm");

        var confirmRequest = new
        {
            ClientName = MockWebApplication.ClientName,
            AccountGroup = MockWebApplication.AccountGroup1,
            Mail = MockWebApplication.Mail,
            Secret = "default",
            SecretToken = existingTickerUser.SecretToken
        };

        request.Content = JsonContent.Create(confirmRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}