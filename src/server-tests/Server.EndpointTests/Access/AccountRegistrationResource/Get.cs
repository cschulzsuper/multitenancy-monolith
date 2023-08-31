﻿using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Server;
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

namespace Access.AccountRegistrationResource;

public sealed class Get : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Get(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Get_ShouldSucceed_WhenExists()
    {
        // Arrange
        var existingAccountRegistration = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountRegistration>>()
                .Insert(existingAccountRegistration);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/access/account-registrations/{existingAccountRegistration.Snowflake}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("accountGroup", existingAccountRegistration.AccountGroup), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("accountMember", existingAccountRegistration.AccountMember), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("authenticationIdentity", existingAccountRegistration.AuthenticationIdentity), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("mailAddress", existingAccountRegistration.MailAddress), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("processState", AccountRegistrationProcessStates.New), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("snowflake", existingAccountRegistration.Snowflake), (x.Key, (long?)x.Value)));
    }

    [Fact]
    public async Task Get_ShouldFail_WhenAbsent()
    {
        // Arrange
        var absentAccountRegistration = 1;

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/access/account-registrations/{absentAccountRegistration}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

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
        var invalidAccountRegistration = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/access/account-registrations/{invalidAccountRegistration}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}