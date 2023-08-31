using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
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

namespace Admission.AuthenticationIdentityResource;

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
        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{existingAuthenticationIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationIdentity = new
        {
            UniqueName = $"put-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "put-info@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var changedIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAuthenticationIdentity.Snowflake &&
                    x.UniqueName == putAuthenticationIdentity.UniqueName &&
                    x.Secret == existingAuthenticationIdentity.Secret &&
                    x.MailAddress == putAuthenticationIdentity.MailAddress);

            Assert.NotNull(changedIdentity);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidAuthenticationIdentity = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{invalidAuthenticationIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationIdentity = new
        {
            UniqueName = "put-authentication-identity",
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationIdentity);

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
        var absentAuthenticationIdentity = "absent-authentication-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{absentAuthenticationIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationIdentity = new
        {
            UniqueName = "put-authentication-identity",
            MailAddress = "info@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationIdentity);

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
        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        var additionalIdentity = new AuthenticationIdentity
        {
            UniqueName = $"additional-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "additional-info@localhost",
            Secret = "additional-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity, additionalIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{existingAuthenticationIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationIdentity = new
        {
            additionalIdentity.UniqueName,
            MailAddress = "put-info@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var unchangedIdentity = scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingAuthenticationIdentity.Snowflake &&
                    x.UniqueName == existingAuthenticationIdentity.UniqueName &&
                    x.Secret == existingAuthenticationIdentity.Secret &&
                    x.MailAddress == existingAuthenticationIdentity.MailAddress);

            Assert.NotNull(unchangedIdentity);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{existingAuthenticationIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationIdentity = new
        {
            UniqueName = (string?)null,
            MailAddress = "put-info@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationIdentity);

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
        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{existingAuthenticationIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationIdentity = new
        {
            UniqueName = string.Empty,
            MailAddress = "put-info@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationIdentity);

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
        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{existingAuthenticationIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationIdentity = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            MailAddress = "put-info@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationIdentity);

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
        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{existingAuthenticationIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationIdentity = new
        {
            UniqueName = "Invalid",
            MailAddress = "put-info@localhost"
        };

        request.Content = JsonContent.Create(putAuthenticationIdentity);

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
        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{existingAuthenticationIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationIdentity = new
        {
            UniqueName = "put-authentication-identity",
            MailAddress = (string?)null
        };

        request.Content = JsonContent.Create(putAuthenticationIdentity);

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
        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{existingAuthenticationIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationIdentity = new
        {
            UniqueName = "put-authentication-identity",
            MailAddress = string.Empty
        };

        request.Content = JsonContent.Create(putAuthenticationIdentity);

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
        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{existingAuthenticationIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationIdentity = new
        {
            UniqueName = "put-authentication-identity",
            MailAddress = $"{new string(Enumerable.Repeat('a', 64).ToArray())}@{new string(Enumerable.Repeat('a', 190).ToArray())}"
        };

        request.Content = JsonContent.Create(putAuthenticationIdentity);

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
        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{existingAuthenticationIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationIdentity = new
        {
            UniqueName = "put-authentication-identity",
            MailAddress = $"{new string(Enumerable.Repeat('a', 65).ToArray())}@{new string(Enumerable.Repeat('a', 1).ToArray())}"
        };

        request.Content = JsonContent.Create(putAuthenticationIdentity);

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
        var existingAuthenticationIdentity = new AuthenticationIdentity
        {
            UniqueName = $"existing-authentication-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<AuthenticationIdentity>>()
                .Insert(existingAuthenticationIdentity);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/admission/authentication-identities/{existingAuthenticationIdentity.UniqueName}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putAuthenticationIdentity = new
        {
            UniqueName = "put-authentication-identity",
            MailAddress = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putAuthenticationIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}