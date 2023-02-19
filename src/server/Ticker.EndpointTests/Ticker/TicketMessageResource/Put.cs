using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ticker.TicketMessageResource;

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Put_ShouldSucceed_WhenPriorityLow()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message-{Guid.NewGuid()}",
            Priority = "low",
            TickerUser = $"{Guid.NewGuid()}@localhost"
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(putTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(putTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(putTickerMessage.Priority, updatedTickerMessage.Priority);
        }
    }

    [Fact]
    public async Task Put_ShouldSucceed_WhenPriorityDefault()
    {
        // Arrange
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "low",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(putTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(putTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(putTickerMessage.Priority, updatedTickerMessage.Priority);
        }
    }

    [Fact]
    public async Task Put_ShouldSucceed_WhenPriorityHigh()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");

        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message-{Guid.NewGuid()}",
            Priority = "high",
            TickerUser = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(putTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(putTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(putTickerMessage.Priority, updatedTickerMessage.Priority);
        }
    }

    [Fact]
    public async Task Put_ShouldSucceed_WhenPriorityCatastrophe()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message-{Guid.NewGuid()}",
            Priority = "catastrophe",
            TickerUser = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(putTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(putTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(putTickerMessage.Priority, updatedTickerMessage.Priority);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidTickerMessages = "invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{invalidTickerMessages}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message-{Guid.NewGuid()}",
            Priority = "catastrophe",
            TickerUser = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerMessage);

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
        var invalidTickerMessages = 1;

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{invalidTickerMessages}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message-{Guid.NewGuid()}",
            Priority = "catastrophe",
            TickerUser = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenTextNull()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = (string?)null,
            Priority = "low",
            TickerUser = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(existingTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(existingTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(existingTickerMessage.Priority, updatedTickerMessage.Priority);
            Assert.Equal(existingTickerMessage.Snowflake, updatedTickerMessage.Snowflake);
            Assert.Equal(existingTickerMessage.Timestamp, updatedTickerMessage.Timestamp);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenTextEmpty()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = string.Empty,
            Priority = "low",
            TickerUser = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(existingTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(existingTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(existingTickerMessage.Priority, updatedTickerMessage.Priority);
            Assert.Equal(existingTickerMessage.Snowflake, updatedTickerMessage.Snowflake);
            Assert.Equal(existingTickerMessage.Timestamp, updatedTickerMessage.Timestamp);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenTextTooLong()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = new string(Enumerable.Repeat('a', 4001).ToArray()),
            Priority = "low",
            TickerUser = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(existingTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(existingTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(existingTickerMessage.Priority, updatedTickerMessage.Priority);
            Assert.Equal(existingTickerMessage.Snowflake, updatedTickerMessage.Snowflake);
            Assert.Equal(existingTickerMessage.Timestamp, updatedTickerMessage.Timestamp);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenPriorityNull()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message",
            Priority = (string?)null,
            TickerUser = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(existingTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(existingTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(existingTickerMessage.Priority, updatedTickerMessage.Priority);
            Assert.Equal(existingTickerMessage.Snowflake, updatedTickerMessage.Snowflake);
            Assert.Equal(existingTickerMessage.Timestamp, updatedTickerMessage.Timestamp);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenPriorityEmpty()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message",
            Priority = string.Empty,
            TickerUser = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(existingTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(existingTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(existingTickerMessage.Priority, updatedTickerMessage.Priority);
            Assert.Equal(existingTickerMessage.Snowflake, updatedTickerMessage.Snowflake);
            Assert.Equal(existingTickerMessage.Timestamp, updatedTickerMessage.Timestamp);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenPriorityInvalid()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message",
            Priority = "invalid",
            TickerUser = MockWebApplication.Mail
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(existingTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(existingTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(existingTickerMessage.Priority, updatedTickerMessage.Priority);
            Assert.Equal(existingTickerMessage.Snowflake, updatedTickerMessage.Snowflake);
            Assert.Equal(existingTickerMessage.Timestamp, updatedTickerMessage.Timestamp);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenTickerUserNull()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message",
            Priority = "low",
            TickerUser = (string?)null
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(existingTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(existingTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(existingTickerMessage.Priority, updatedTickerMessage.Priority);
            Assert.Equal(existingTickerMessage.Snowflake, updatedTickerMessage.Snowflake);
            Assert.Equal(existingTickerMessage.Timestamp, updatedTickerMessage.Timestamp);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenTickerUserEmpty()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message",
            Priority = "low",
            TickerUser = string.Empty
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(existingTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(existingTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(existingTickerMessage.Priority, updatedTickerMessage.Priority);
            Assert.Equal(existingTickerMessage.Snowflake, updatedTickerMessage.Snowflake);
            Assert.Equal(existingTickerMessage.Timestamp, updatedTickerMessage.Timestamp);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenTickerUserTooLong()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message",
            Priority = "low",
            TickerUser = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(existingTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(existingTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(existingTickerMessage.Priority, updatedTickerMessage.Priority);
            Assert.Equal(existingTickerMessage.Snowflake, updatedTickerMessage.Snowflake);
            Assert.Equal(existingTickerMessage.Timestamp, updatedTickerMessage.Timestamp);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenTickerUserLocalPartTooLong()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message",
            Priority = "low",
            TickerUser = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(existingTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(existingTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(existingTickerMessage.Priority, updatedTickerMessage.Priority);
            Assert.Equal(existingTickerMessage.Snowflake, updatedTickerMessage.Snowflake);
            Assert.Equal(existingTickerMessage.Timestamp, updatedTickerMessage.Timestamp);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenTickerUserInvalid()
    {
        // Arrange
        var existingTickerMessage = new TickerMessage
        {
            Snowflake = 1,
            Text = $"existing-ticker-message-{Guid.NewGuid()}",
            Priority = "default",
            TickerUser = $"{Guid.NewGuid()}@localhost",
            Timestamp = 0
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .Insert(existingTickerMessage);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/ticker/ticker-messages/{existingTickerMessage.Snowflake}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var putTickerMessage = new
        {
            Text = $"put-ticker-message",
            Priority = "low",
            TickerUser = "Invalid"
        };

        request.Content = JsonContent.Create(putTickerMessage);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
        {
            var updatedTickerMessage = scope.ServiceProvider
                .GetRequiredService<IRepository<TickerMessage>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedTickerMessage);
            Assert.Equal(existingTickerMessage.TickerUser, updatedTickerMessage.TickerUser);
            Assert.Equal(existingTickerMessage.Text, updatedTickerMessage.Text);
            Assert.Equal(existingTickerMessage.Priority, updatedTickerMessage.Priority);
            Assert.Equal(existingTickerMessage.Snowflake, updatedTickerMessage.Snowflake);
            Assert.Equal(existingTickerMessage.Timestamp, updatedTickerMessage.Timestamp);
        }
    }
}