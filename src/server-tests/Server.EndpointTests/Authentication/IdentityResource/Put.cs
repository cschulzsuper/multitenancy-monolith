using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
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

namespace Authentication.IdentityResource;

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
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

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
            var changedIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingIdentity.Snowflake &&
                    x.UniqueName == putIdentity.UniqueName &&
                    x.Secret == putIdentity.Secret &&
                    x.MailAddress == putIdentity.MailAddress);

            Assert.NotNull(changedIdentity);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidIdentity = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{invalidIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putIdentity = new
        {
            UniqueName = "put-identity",
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

    [Fact]
    public async Task Put_ShouldFail_WhenAbsent()
    {
        // Arrange
        var absentIdentity = "absent-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{absentIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putIdentity = new
        {
            UniqueName = "put-identity",
            MailAddress = "info@localhost",
            Secret = "foo-bar"
        };

        request.Content = JsonContent.Create(putIdentity);

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
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        var additionalIdentity = new Identity
        {
            UniqueName = $"additional-identity-{Guid.NewGuid()}",
            MailAddress = "additional-info@localhost",
            Secret = "additional-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .Insert(existingIdentity, additionalIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{existingIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putIdentity = new
        {
            additionalIdentity.UniqueName,
            MailAddress = "put-info@localhost",
            Secret = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var unchangedIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<Identity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingIdentity.Snowflake &&
                    x.UniqueName == existingIdentity.UniqueName &&
                    x.Secret == existingIdentity.Secret &&
                    x.MailAddress == existingIdentity.MailAddress);

            Assert.NotNull(unchangedIdentity);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

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
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameEmpty()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

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
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameTooLong()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putIdentity = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
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
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameInvalid()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putIdentity = new
        {
            UniqueName = "Invalid",
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
    }

    [Fact]
    public async Task Put_ShouldFail_WhenSecretNull()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putIdentity = new
        {
            UniqueName = "put-identity",
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
    }

    [Fact]
    public async Task Put_ShouldFail_WhenSecretEmpty()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putIdentity = new
        {
            UniqueName = "put-identity",
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
    }

    [Fact]
    public async Task Put_ShouldFail_WhenSecretTooLong()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putIdentity = new
        {
            UniqueName = "put-identity",
            MailAddress = "put-info@localhost",
            Secret = new string(Enumerable.Repeat('a', 141).ToArray()),
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressNull()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putIdentity = new
        {
            UniqueName = "put-identity",
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
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressEmpty()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putIdentity = new
        {
            UniqueName = "put-identity",
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
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressTooLong()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putIdentity = new
        {
            UniqueName = "put-identity",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}",
            Secret = "put-foo-bar",
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressLocalPartTooLong()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putIdentity = new
        {
            UniqueName = "put-identity",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}",
            Secret = "put-foo-bar",
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenMailAddressInvalid()
    {
        // Arrange
        var existingIdentity = new Identity
        {
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putIdentity = new
        {
            UniqueName = "put-identity",
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
    }
}