using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ticker.TickerUserResource;

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
        var existingTickerUser = new TickerUser
        {
            DisplayName = "Exiting Test User",
            MailAddress = $"{Guid.NewGuid()}@localhost",
            Secret = $"{Guid.NewGuid()}",
            SecretState = TickerUserSecretStates.Confirmed,
            SecretToken = Guid.NewGuid()
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(existingTickerUser);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-users/{existingTickerUser.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerUser);
            Assert.Equal(putTickerUser.DisplayName, updatedTickerUser.DisplayName);
            Assert.Equal(putTickerUser.MailAddress, updatedTickerUser.MailAddress);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidTickerUsers = "invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-users/{invalidTickerUsers}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAbsent()
    {
        // Arrange
        var invalidTickerUsers = 1;

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-users/{invalidTickerUsers}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenDisplayNameNull()
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

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(existingTickerUser);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-users/{existingTickerUser.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerUser = new
        {
            DisplayName = (string?)null,
            MailAddress = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerUser);
            Assert.Equal(existingTickerUser.DisplayName, updatedTickerUser.DisplayName);
            Assert.Equal(existingTickerUser.MailAddress, updatedTickerUser.MailAddress);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenDisplayNameEmpty()
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

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(existingTickerUser);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-users/{existingTickerUser.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerUser = new
        {
            DisplayName = string.Empty,
            MailAddress = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerUser);
            Assert.Equal(existingTickerUser.DisplayName, updatedTickerUser.DisplayName);
            Assert.Equal(existingTickerUser.MailAddress, updatedTickerUser.MailAddress);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenDisplayNameTooLong()
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

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(existingTickerUser);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-users/{existingTickerUser.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerUser = new
        {
            DisplayName = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerUser);
            Assert.Equal(existingTickerUser.DisplayName, updatedTickerUser.DisplayName);
            Assert.Equal(existingTickerUser.MailAddress, updatedTickerUser.MailAddress);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressNull()
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

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(existingTickerUser);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-users/{existingTickerUser.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerUser = new
        {
            DisplayName = $"put-ticker-user",
            MailAddress = (string?)null
        };

        request.Content = JsonContent.Create(putTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerUser);
            Assert.Equal(existingTickerUser.DisplayName, updatedTickerUser.DisplayName);
            Assert.Equal(existingTickerUser.MailAddress, updatedTickerUser.MailAddress);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressEmpty()
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

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(existingTickerUser);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-users/{existingTickerUser.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerUser = new
        {
            DisplayName = $"put-ticker-user",
            MailAddress = string.Empty
        };

        request.Content = JsonContent.Create(putTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerUser);
            Assert.Equal(existingTickerUser.DisplayName, updatedTickerUser.DisplayName);
            Assert.Equal(existingTickerUser.MailAddress, updatedTickerUser.MailAddress);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressTooLong()
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

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(existingTickerUser);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-users/{existingTickerUser.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerUser = new
        {
            DisplayName = $"put-ticker-user",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(putTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerUser);
            Assert.Equal(existingTickerUser.DisplayName, updatedTickerUser.DisplayName);
            Assert.Equal(existingTickerUser.MailAddress, updatedTickerUser.MailAddress);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressLocalPartTooLong()
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

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(existingTickerUser);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-users/{existingTickerUser.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerUser = new
        {
            DisplayName = $"put-ticker-user",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(putTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerUser);
            Assert.Equal(existingTickerUser.DisplayName, updatedTickerUser.DisplayName);
            Assert.Equal(existingTickerUser.MailAddress, updatedTickerUser.MailAddress);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressInvalid()
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

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(existingTickerUser);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-users/{existingTickerUser.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerUser = new
        {
            DisplayName = $"put-ticker-user",
            MailAddress = "Invalid"
        };

        request.Content = JsonContent.Create(putTickerUser);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerUser = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerUser);
            Assert.Equal(existingTickerUser.DisplayName, updatedTickerUser.DisplayName);
            Assert.Equal(existingTickerUser.MailAddress, updatedTickerUser.MailAddress);
        }
    }
}