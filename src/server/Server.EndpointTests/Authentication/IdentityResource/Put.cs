using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Authentication.IdentityResource;

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Fact]
    [Trait("Category", "Endpoint.Security")]
    public async Task Put_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");

        var putIdentity = new
        {
            UniqueName = "put-identity",
            MailAddress = "put-info@localhost",
            Secret = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [Trait("Category", "Endpoint.Security")]
    [InlineData(TestConfiguration.ChiefIdentity)]
    [InlineData(TestConfiguration.DefaultIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Put_ShouldBeForbidden_WhenNotAdmin(string identity)
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = "put-identity",
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
    public async Task Put_ShouldSucceed_WhenValid(string identity)
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenInvalid(string identity)
    {
        // Arrange
        var invalidIdentity = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{invalidIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenAbsent(string identity)
    {
        // Arrange
        var absentIdentity = "absent-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{absentIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenUniqueNameExists(string identity)
    {
        // Arrange
        var existingIdentity = new Identity
        {
            Snowflake = 1,
            UniqueName = $"existing-identity-{Guid.NewGuid()}",
            MailAddress = "existing-info@localhost",
            Secret = "existing-foo-bar"
        };

        var additionalIdentity = new Identity
        {
            Snowflake = 2,
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
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = additionalIdentity.UniqueName,
            MailAddress = "put-info@localhost",
            Secret = "put-foo-bar"
        };

        request.Content = JsonContent.Create(putIdentity);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenUniqueNameNull(string identity)
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = (string?) null,
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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenUniqueNameEmpty(string identity)
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");
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
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenUniqueNameTooLong(string identity)
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenUniqueNameInvalid(string identity)
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenSecretNull(string identity)
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = "put-identity",
            MailAddress = "put-info@localhost",
            Secret = (string?) null,
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
    public async Task Put_ShouldFail_WhenSecretEmpty(string identity)
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenSecretTooLong(string identity)
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenMailAddressNull(string identity)
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var putIdentity = new
        {
            UniqueName = "put-identity",
            MailAddress = (string?) null,
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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenMailAddressEmpty(string identity)
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

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

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.AdminIdentity)]
    public async Task Put_ShouldFail_WhenMailAddressInvalid(string identity)
    {
        // Arrange
        var validIdentity = "valid-identity";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authentication/identities/{validIdentity}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

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