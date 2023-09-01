using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
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

namespace Admission.AuthenticationIdentityResource;

public sealed class GetAll : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetAll(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task GetAll_ShouldSucceed()
    {
        // Arrange
        var existingAuthenticationIdentity1 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-1-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        var existingAuthenticationIdentity2 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-2-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity1, existingAuthenticationIdentity2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/admission/authentication-identities");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
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

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity1, existingAuthenticationIdentity2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/admission/authentication-identities?q=mail-address:info1@localhost");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x =>
            {
                Assert.Equal(existingAuthenticationIdentity1.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingAuthenticationIdentity1.UniqueName, x.GetString("uniqueName"));
            });
    }

    [Fact]
    public async Task GetAll_ShouldRespectQueryUniqueName()
    {
        // Arrange
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

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity1, existingAuthenticationIdentity2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/admission/authentication-identities?q=unique-name:{existingAuthenticationIdentity1.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x =>
            {
                Assert.Equal(existingAuthenticationIdentity1.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingAuthenticationIdentity1.UniqueName, x.GetString("uniqueName"));
            });
    }

    [Fact]
    public async Task GetAll_ShouldRespectTake()
    {
        // Arrange
        var existingAuthenticationIdentity1 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-1-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        var existingAuthenticationIdentity2 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-2-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity1, existingAuthenticationIdentity2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/admission/authentication-identities?t=1");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x =>
            {
                Assert.Equal(existingAuthenticationIdentity1.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingAuthenticationIdentity1.UniqueName, x.GetString("uniqueName"));
            });
    }

    [Fact]
    public async Task GetAll_ShouldRespectSkip()
    {
        // Arrange
        var existingAuthenticationIdentity1 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-1-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        var existingAuthenticationIdentity2 = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-2-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity1, existingAuthenticationIdentity2);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/a1/admission/authentication-identities?s=1");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x =>
            {
                Assert.Equal(existingAuthenticationIdentity2.MailAddress, x.GetString("mailAddress"));
                Assert.Equal(existingAuthenticationIdentity2.UniqueName, x.GetString("uniqueName"));
            });
    }
}