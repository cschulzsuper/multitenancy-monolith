using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Access.ContextAccountRegistrationCommands;

public sealed class Confirm : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Confirm(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Theory]
    [InlineData(MockWebApplication.MockAdmin)]
    [InlineData(MockWebApplication.MockIdentity)]
    [InlineData(MockWebApplication.MockDemo)]
    [InlineData(MockWebApplication.MockChief)]
    [InlineData(MockWebApplication.MockChiefObserver)]
    [InlineData(MockWebApplication.MockMember)]
    [InlineData(MockWebApplication.MockMemberObserver)]
    public async Task Confirm_ShouldFail_WhenNotAuthenticated(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-registrations/_/confirm");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);
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
    public async Task Confirm_ShouldFail_WhenAuthorized(int mock)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-registrations/_/confirm");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);
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
    public async Task Confirm_ShouldBeForbidden_WhenProcessTokenInvalid(int mock)
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AuthenticationIdentity = $"existing-account-registration-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            AccountGroup = $"register-account-registration-group-{Guid.NewGuid()}",
            AccountMember = $"register-account-registration-member-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid()
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-registrations/_/confirm");
        request.Headers.Authorization = _factory.MockValidAuthorizationHeader(mock);

        var confirmRequest = new
        {
            existingAccountRegistration.AccountGroup,
            ProcessToken = Guid.NewGuid()
        };

        request.Content = JsonContent.Create(confirmRequest);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
    }

    [Fact]
    public async Task Register_ShouldBeUnauthorized_WhenInvalid()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/a1/access/account-registrations/_/confirm");
        request.Headers.Authorization = _factory.MockInvalidAuthorizationHeader();
        request.Content = JsonContent.Create(new object());

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }
}