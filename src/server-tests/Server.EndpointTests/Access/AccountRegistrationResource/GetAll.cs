﻿using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Server;
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

namespace Access.AccountRegistrationResource;

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
        var existingRegistration1 = new AccountRegistration
        {
            AccountGroup = $"existing-account-registration-account-group-{Guid.NewGuid()}",
            AccountMember = $"existing-account-registration-account-member-{Guid.NewGuid()}",
            AuthenticationIdentity = $"existing-account-registration-authentication-identity-{Guid.NewGuid()}",
            ProcessState = AccountRegistrationProcessStates.New,
            ProcessToken = Guid.NewGuid(),
            MailAddress = "default@localhost"
        };

        var existingRegistration2 = new AccountRegistration
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
                .Insert(existingRegistration1, existingRegistration2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/access/account-registrations");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("snowflake")),
            x =>
            {
                Assert.Equal(existingRegistration1.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingRegistration1.Snowflake, x.GetProperty("snowflake").GetInt64());
                Assert.Equal(existingRegistration1.AuthenticationIdentity, x.GetString("authenticationIdentity"));
                Assert.Equal(existingRegistration1.AccountGroup, x.GetString("accountGroup"));
                Assert.Equal(existingRegistration1.AccountMember, x.GetString("accountMember"));
                Assert.Equal(AccountRegistrationProcessStates.New, x.GetString("processState"));
            },
            x =>
            {
                Assert.Equal(existingRegistration2.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingRegistration2.Snowflake, x.GetProperty("snowflake").GetInt64());
                Assert.Equal(existingRegistration2.AuthenticationIdentity, x.GetString("authenticationIdentity"));
                Assert.Equal(existingRegistration2.AccountGroup, x.GetString("accountGroup"));
                Assert.Equal(existingRegistration2.AccountMember, x.GetString("accountMember"));
                Assert.Equal(AccountRegistrationProcessStates.New, x.GetString("processState"));
            });
    }
}