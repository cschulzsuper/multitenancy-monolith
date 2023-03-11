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
using ChristianSchulz.MultitenancyMonolith.Events;
using System.Threading;

namespace Ticker.TickerUserFlows;

public sealed class SecretFlow : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private Action<string>? _eventPublicationInterceptorAssert = null;
    private TaskCompletionSource _eventPublicationInterceptorTask = new TaskCompletionSource();

    public SecretFlow(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
        _factory.Services.GetRequiredService<EventsOptions>()
            .PublicationInterceptor = (scope, @event, snowflake) =>
            {
                _eventPublicationInterceptorAssert?.Invoke(@event);
                _eventPublicationInterceptorTask.SetResult();
            };
    }

    [Fact]
    public async Task Exceute()
    {
        await TickerUser_Create_ShouldSucceed();
        await TickerUser_Reset_ShouldSucceed();

        await TickerUser_Auth_ShouldSucceed_WithSecretStatePending();
        await TickerUser_Auth_ShouldBeForbidden();

        await TickerUser_Confirm_ShouldSucceed_WithSecretStateConfirmed();
        await TickerUser_Confirm_ShouldFail();

        await TickerUser_Auth_ShouldSucceed_WithSecretStateConfirmed();
        await TickerUser_Auth_ShouldSucceed_WithSecretStateConfirmed();
    }

    private async Task TickerUser_Create_ShouldSucceed()
    {
        // Arrange
        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users");
        createRequest.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var createContent = new
        {
            DisplayName = "Default",
            MailAddress = MockWebApplication.TickerUserMail
        };

        createRequest.Content = JsonContent.Create(createContent);

        var client = _factory.CreateClient();

        // Act
        var authResponse = await client.SendAsync(createRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, authResponse.StatusCode);

        using var scope = _factory.CreateMultitenancyScope();

        var createdTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.NotNull(createdTickerUser);
        Assert.Equal(TickerUserSecretStates.Invalid, createdTickerUser.SecretState);
    }

    private async Task TickerUser_Reset_ShouldSucceed()
    {
        // Arrange
        using var scope = _factory.CreateMultitenancyScope();

        var existingTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .Single();

        _eventPublicationInterceptorAssert = @event => Assert.Equal("ticker-user-secret-reset", @event);
        _eventPublicationInterceptorTask = new TaskCompletionSource();

        var cancellationTokenSource = new CancellationTokenSource(2000);
        cancellationTokenSource.Token.Register(_eventPublicationInterceptorTask.SetCanceled);

        var resetRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/{existingTickerUser.Snowflake}/reset");
        resetRequest.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(resetRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resetTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.NotNull(resetTickerUser);
        Assert.Equal(TickerUserSecretStates.Reset, resetTickerUser.SecretState);

        await _eventPublicationInterceptorTask.Task;
        Assert.True(_eventPublicationInterceptorTask.Task.IsCompletedSuccessfully);
    }

    private async Task TickerUser_Auth_ShouldSucceed_WithSecretStatePending()
    {
        // Arrange
        _eventPublicationInterceptorAssert = @event => Assert.Equal("ticker-user-secret-pending", @event);
        _eventPublicationInterceptorTask = new TaskCompletionSource();

        var cancellationTokenSource = new CancellationTokenSource(2000);
        cancellationTokenSource.Token.Register(_eventPublicationInterceptorTask.SetCanceled);

        var authRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/_/auth");
        var authContent = new
        {
            ClientName = MockWebApplication.ClientName,
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = MockWebApplication.TickerUserMail,
            Secret = MockWebApplication.TickerUserSecret
        };

        authRequest.Content = JsonContent.Create(authContent);

        var client = _factory.CreateClient();

        // Act
        var authResponse = await client.SendAsync(authRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, authResponse.StatusCode);

        using var scope = _factory.CreateMultitenancyScope();

        var updatedTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.NotNull(updatedTickerUser);
        Assert.Equal(TickerUserSecretStates.Pending, updatedTickerUser.SecretState);
        Assert.NotEqual(MockWebApplication.TickerUserSecretToken, updatedTickerUser.SecretToken);

        await _eventPublicationInterceptorTask.Task;
        Assert.True(_eventPublicationInterceptorTask.Task.IsCompletedSuccessfully);
    }

    private async Task TickerUser_Auth_ShouldBeForbidden()
    {
        // Arrange
        var authRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/_/auth");
        var authContent = new
        {
            ClientName = MockWebApplication.ClientName,
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = MockWebApplication.TickerUserMail,
            Secret = MockWebApplication.TickerUserSecret
        };

        authRequest.Content = JsonContent.Create(authContent);

        var client = _factory.CreateClient();

        // Act
        var authResponse = await client.SendAsync(authRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, authResponse.StatusCode);
    }

    private async Task TickerUser_Confirm_ShouldSucceed_WithSecretStateConfirmed()
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

        var authRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/_/confirm");
        var authContent = new
        {
            ClientName = MockWebApplication.ClientName,
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = MockWebApplication.TickerUserMail,
            Secret = MockWebApplication.TickerUserSecret,
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
            var updatedTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerUser);
            Assert.Equal(TickerUserSecretStates.Confirmed, updatedTickerUser.SecretState);
            Assert.NotEqual(Guid.Empty, updatedTickerUser.SecretToken);
        }
    }

    private async Task TickerUser_Confirm_ShouldFail()
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

        var authRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/_/confirm");
        var authContent = new
        {
            ClientName = MockWebApplication.ClientName,
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = MockWebApplication.TickerUserMail,
            Secret = MockWebApplication.TickerUserSecret,
            SecretToken = tickerUserSecretToken
        };

        authRequest.Content = JsonContent.Create(authContent);

        var client = _factory.CreateClient();

        // Act
        var authResponse = await client.SendAsync(authRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, authResponse.StatusCode);
    }

    private async Task TickerUser_Auth_ShouldSucceed_WithSecretStateConfirmed()
    {
        // Arrange
        var authRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/ticker/ticker-users/_/auth");
        var authContent = new
        {
            ClientName = MockWebApplication.ClientName,
            AccountGroup = MockWebApplication.AccountGroup,
            Mail = MockWebApplication.TickerUserMail,
            Secret = MockWebApplication.TickerUserSecret
        };

        authRequest.Content = JsonContent.Create(authContent);

        var client = _factory.CreateClient();

        // Act
        var authResponse = await client.SendAsync(authRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, authResponse.StatusCode);

        using var scope = _factory.CreateMultitenancyScope();

        var updatedTickerUser = scope.ServiceProvider
            .GetRequiredService<IRepository<TickerUser>>()
            .GetQueryable()
            .SingleOrDefault();

        Assert.NotNull(updatedTickerUser);
        Assert.Equal(TickerUserSecretStates.Confirmed, updatedTickerUser.SecretState);
        Assert.NotEqual(Guid.Empty, updatedTickerUser.SecretToken);
    }
}