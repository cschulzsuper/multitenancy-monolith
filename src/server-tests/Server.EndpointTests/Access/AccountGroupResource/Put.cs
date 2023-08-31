using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Access.AccountGroupResource;

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Put_ShouldSucceed_WhenValid()
    {
        // Arrange
        var existingAccountGroup = new AccountGroup
        {
            UniqueName = $"existing-account-group-{Guid.NewGuid()}"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountGroup>>()
                .Insert(existingAccountGroup);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-groups/{existingAccountGroup.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountGroup = new
        {
            UniqueName = $"put-account-group-{Guid.NewGuid()}"
        };

        request.Content = JsonContent.Create(putAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var changedIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountGroup>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountGroup.Snowflake &&
                    x.UniqueName == putAccountGroup.UniqueName);

            Assert.NotNull(changedIdentity);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidAccountGroup = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-groups/{invalidAccountGroup}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountGroup = new
        {
            UniqueName = "put-account-group"
        };

        request.Content = JsonContent.Create(putAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAbsent()
    {
        // Arrange
        var absentAccountGroup = "absent-account-group";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-groups/{absentAccountGroup}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountGroup = new
        {
            UniqueName = "put-account-group"
        };

        request.Content = JsonContent.Create(putAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameExists()
    {
        // Arrange
        var existingAccountGroup = new AccountGroup
        {
            UniqueName = $"existing-account-group-{Guid.NewGuid()}"
        };

        var additionalIdentity = new AccountGroup
        {
            UniqueName = $"additional-account-group-{Guid.NewGuid()}"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountGroup>>()
                .Insert(existingAccountGroup, additionalIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-groups/{existingAccountGroup.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountGroup = new
        {
            additionalIdentity.UniqueName,
            MailAddress = "put-info@localhost"
        };

        request.Content = JsonContent.Create(putAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var unchangedIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountGroup>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAccountGroup.Snowflake &&
                    x.UniqueName == existingAccountGroup.UniqueName);

            Assert.NotNull(unchangedIdentity);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        var existingAccountGroup = new AccountGroup
        {
            UniqueName = $"existing-account-group-{Guid.NewGuid()}"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountGroup>>()
                .Insert(existingAccountGroup);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-groups/{existingAccountGroup.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountGroup = new
        {
            UniqueName = (string?)null
        };

        request.Content = JsonContent.Create(putAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameEmpty()
    {
        // Arrange
        var existingAccountGroup = new AccountGroup
        {
            UniqueName = $"existing-account-group-{Guid.NewGuid()}"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountGroup>>()
                .Insert(existingAccountGroup);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-groups/{existingAccountGroup.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountGroup = new
        {
            UniqueName = string.Empty
        };

        request.Content = JsonContent.Create(putAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameTooLong()
    {
        // Arrange
        var existingAccountGroup = new AccountGroup
        {
            UniqueName = $"existing-account-group-{Guid.NewGuid()}"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountGroup>>()
                .Insert(existingAccountGroup);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-groups/{existingAccountGroup.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountGroup = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray())
        };

        request.Content = JsonContent.Create(putAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameInvalid()
    {
        // Arrange
        var existingAccountGroup = new AccountGroup
        {
            UniqueName = $"existing-account-group-{Guid.NewGuid()}"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountGroup>>()
                .Insert(existingAccountGroup);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/access/account-groups/{existingAccountGroup.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAccountGroup = new
        {
            UniqueName = "Invalid"
        };

        request.Content = JsonContent.Create(putAccountGroup);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}