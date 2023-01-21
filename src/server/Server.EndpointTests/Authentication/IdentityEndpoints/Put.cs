using ChristianSchulz.MultitenancyMonolith.Aggregates.Authentication;
using ChristianSchulz.MultitenancyMonolith.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Authentication.IdentityEndpoints;

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [Trait("Category", "Endpoint.Security")]
    [InlineData(TestConfiguration.ChiefIdentity)]
    [InlineData(TestConfiguration.DefaultIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Delete_ShouldBeForbidden_WhenIdentityIsNotAdmin(string identity)
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName =  $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{existingIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = $"put-identity-{Guid.NewGuid()}",
            MailAddress = "put-info@localhost",
            Secret = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldSucceed_WhenValidExistingIdentityIsGiven(string identity)
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName =  $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{existingIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = $"put-identity-{Guid.NewGuid()}",
            MailAddress = "put-info@localhost",
            Secret = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var createdIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingIdentity.Snowflake &&
                    x.UniqueName == putIdentity.UniqueName &&
                    x.Secret == putIdentity.Secret &&
                    x.MailAddress == putIdentity.MailAddress);

            Assert.NotNull(createdIdentity);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenNotExistingIdentityIsGiven(string identity)
    {
        // Arrange
        var notExistingIdentityUniqueName = $"not-existing-identity-{Guid.NewGuid()}";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{notExistingIdentityUniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = $"put-identity-{Guid.NewGuid()}",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenIdentityUniqueNameIsNull(string identity)
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName =  $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{existingIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = (string?)null,
            MailAddress = "put-info@localhost",
            Secret = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateScope())
        {
            var createdIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingIdentity.Snowflake &&
                    (x.UniqueName == putIdentity.UniqueName ||
                     x.MailAddress == putIdentity.MailAddress ||
                     x.Secret == putIdentity.Secret));

            Assert.Null(createdIdentity);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenIdentityUniqueNameIsEmpty(string identity)
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName =  $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{existingIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = string.Empty,
            MailAddress = "put-info@localhost",
            Secret = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateScope())
        {
            var createdIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingIdentity.Snowflake &&
                    (x.UniqueName == putIdentity.UniqueName ||
                     x.MailAddress == putIdentity.MailAddress ||
                     x.Secret == putIdentity.Secret));

            Assert.Null(createdIdentity);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenIdentitySecretIsNull(string identity)
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName =  $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{existingIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = $"put-member-{Guid.NewGuid()}",
            MailAddress = "put-info@localhost",
            Secret = (string?)null,
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateScope())
        {
            var createdIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingIdentity.Snowflake &&
                    (x.UniqueName == putIdentity.UniqueName ||
                     x.MailAddress == putIdentity.MailAddress ||
                     x.Secret == putIdentity.Secret));

            Assert.Null(createdIdentity);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenIdentitySecretIsEmpty(string identity)
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName =  $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{existingIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = $"put-member-{Guid.NewGuid()}",
            MailAddress = "put-info@localhost",
            Secret = string.Empty,
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateScope())
        {
            var createdIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingIdentity.Snowflake &&
                    (x.UniqueName == putIdentity.UniqueName ||
                     x.MailAddress == putIdentity.MailAddress ||
                     x.Secret == putIdentity.Secret));

            Assert.Null(createdIdentity);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenIdentityMailAddressIsEmpty(string identity)
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName =  $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{existingIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = $"put-member-{Guid.NewGuid()}",
            MailAddress = string.Empty,
            Secret = "put-foo-bar",
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateScope())
        {
            var createdIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingIdentity.Snowflake &&
                    (x.UniqueName == putIdentity.UniqueName ||
                     x.MailAddress == putIdentity.MailAddress ||
                     x.Secret == putIdentity.Secret));

            Assert.Null(createdIdentity);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenIdentityMailAddressIsNull(string identity)
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName =  $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{existingIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = $"put-member-{Guid.NewGuid()}",
            MailAddress = (string?)null,
            Secret = "put-foo-bar",
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateScope())
        {
            var createdIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingIdentity.Snowflake &&
                    (x.UniqueName == putIdentity.UniqueName ||
                     x.MailAddress == putIdentity.MailAddress ||
                     x.Secret == putIdentity.Secret));

            Assert.Null(createdIdentity);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenIdentityMailAddressIsNotMailAddress(string identity)
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName =  $"existing-member-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{existingIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = $"put-member-{Guid.NewGuid()}",
            MailAddress = "put-foo-bar",
            Secret = "put-foo-bar",
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateScope())
        {
            var createdIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingIdentity.Snowflake &&
                    (x.UniqueName == putIdentity.UniqueName ||
                     x.MailAddress == putIdentity.MailAddress ||
                     x.Secret == putIdentity.Secret));

            Assert.Null(createdIdentity);
        }
    }
}