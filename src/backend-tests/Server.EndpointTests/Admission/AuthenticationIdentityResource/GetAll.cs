﻿using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Admission.AuthenticationIdentityResource;

public sealed class GetAll 
{
    [Fact]
    public async Task GetAll_ShouldSucceed()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAuthenticationIdentity1 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-1-{Guid.NewGuid()}",
            MailAddress = "info1@localhost",
            Secret = "foo-bar"
        };

        var existingAuthenticationIdentity2 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-2-{Guid.NewGuid()}",
            MailAddress = "info2@localhost",
            Secret = "foo-bar"
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity1, existingAuthenticationIdentity2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/admission/authentication-identities");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray()
            .Where(x => 
                x.GetString("mailAddress") == "info1@localhost" || 
                x.GetString("mailAddress") == "info2@localhost")
            .OrderBy(x => x.GetString("uniqueName")),
            x =>
            {
                Assert.Equal(existingAuthenticationIdentity1.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingAuthenticationIdentity1.UniqueName, x.GetString("uniqueName"));
            },
            x =>
            {
                Assert.Equal(existingAuthenticationIdentity2.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingAuthenticationIdentity2.UniqueName, x.GetString("uniqueName"));
            });
    }

    [Fact]
    public async Task GetAll_ShouldRespectQueryMailAddress()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAuthenticationIdentity1 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-3-{Guid.NewGuid()}",
            MailAddress = "info3@localhost",
            Secret = "foo-bar"
        };

        var existingAuthenticationIdentity2 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-4-{Guid.NewGuid()}",
            MailAddress = "info4@localhost",
            Secret = "foo-bar"
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity1, existingAuthenticationIdentity2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/admission/authentication-identities?q=mail-address:info3@localhost");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Single(content.RootElement.EnumerateArray(),
            x => existingAuthenticationIdentity1.MailAddress == x.GetString("mailAddress") &&
                 existingAuthenticationIdentity1.UniqueName == x.GetString("uniqueName"));
    }

    [Fact]
    public async Task GetAll_ShouldRespectQueryUniqueName()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAuthenticationIdentity1 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-5-{Guid.NewGuid()}",
            MailAddress = "info5@localhost",
            Secret = "foo-bar"
        };

        var existingAuthenticationIdentity2 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-6-{Guid.NewGuid()}",
            MailAddress = "info6@localhost",
            Secret = "foo-bar"
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity1, existingAuthenticationIdentity2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/admission/authentication-identities?q=unique-name:{existingAuthenticationIdentity1.UniqueName}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Single(content.RootElement.EnumerateArray(),
            x => existingAuthenticationIdentity1.MailAddress == x.GetString("mailAddress") &&
                 existingAuthenticationIdentity1.UniqueName == x.GetString("uniqueName"));
    }

    [Fact]
    public async Task GetAll_ShouldRespectTake()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAuthenticationIdentity1 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-7-{Guid.NewGuid()}",
            MailAddress = "info7@localhost",
            Secret = "foo-bar"
        };

        var existingAuthenticationIdentity2 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-8-{Guid.NewGuid()}",
            MailAddress = "info7@localhost",
            Secret = "foo-bar"
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity1, existingAuthenticationIdentity2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/admission/authentication-identities?t=1&q=mail-address:info7@localhost");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Single(content.RootElement.EnumerateArray(), 
            x => existingAuthenticationIdentity1.MailAddress == x.GetString("mailAddress") &&
                 existingAuthenticationIdentity1.UniqueName == x.GetString("uniqueName"));
    }

    [Fact]
    public async Task GetAll_ShouldRespectSkip()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingAuthenticationIdentity1 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-10-{Guid.NewGuid()}",
            MailAddress = "info9@localhost",
            Secret = "foo-bar"
        };

        var existingAuthenticationIdentity2 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-11-{Guid.NewGuid()}",
            MailAddress = "info9@localhost",
            Secret = "foo-bar"
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity1, existingAuthenticationIdentity2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/admission/authentication-identities?s=1&q=mail-address:info9@localhost");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Single(content.RootElement.EnumerateArray(),
            x => existingAuthenticationIdentity2.MailAddress == x.GetString("mailAddress") &&
                 existingAuthenticationIdentity2.UniqueName == x.GetString("uniqueName"));
    }
}