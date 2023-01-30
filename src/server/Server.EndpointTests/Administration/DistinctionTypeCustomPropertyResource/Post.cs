using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.DistinctionTypeCustomPropertyResource;

public sealed class Post : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Post(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Fact]
    [Trait("Category", "Endpoint.Security")]
    public async Task Post_ShouldBeUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var validDistinctionType = "required-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/distinction-types/{validDistinctionType}/custom-properties");

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = "post-distinction-type-custom-property"
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [Trait("Category", "Endpoint.Security")]
    [InlineData(TestConfiguration.AdminIdentity)]
    [InlineData(TestConfiguration.DefaultIdentity)]
    [InlineData(TestConfiguration.GuestIdentity)]
    public async Task Post_ShouldBeForbidden_WhenNotAuthorized(string identity)
    {
        // Arrange
        var validDistinctionType = "required-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/distinction-types/{validDistinctionType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = "post-distinction-type-custom-property"
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [Trait("Category", "Endpoint.Security")]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.DefaultIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    public async Task Post_ShouldBeForbidden_WhenNotChief(string identity, string group, string member)
    {
        // Arrange
        var validDistinctionType = "required-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/distinction-types/{validDistinctionType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = "post-distinction-type-custom-property"
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldSucceed_WhenValid(string identity, string group, string member)
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = $"post-distinction-type-custom-property-{Guid.NewGuid()}"
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal((x.Key, (string?) x.Value), ("distinctionType", existingDistinctionType.UniqueName)),
            x => Assert.Equal((x.Key, (string?) x.Value), ("uniqueName", postDistinctionTypeCustomProperty.UniqueName)));

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var createdDistinctionTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .Single(x => x.UniqueName == existingDistinctionType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == postDistinctionTypeCustomProperty.UniqueName);

            Assert.NotNull(createdDistinctionTypeCustomProperty);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenInvalidDistinctionType(string identity, string group, string member)
    {
        // Arrange
        var invalidDistinctionType = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/distinction-types/{invalidDistinctionType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = "post-distinction-type-custom-property"
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameExists(string identity, string group, string member)
    {
        // Arrange
        var existingDistinctionTypeCustomProperty = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property-{Guid.NewGuid()}"
        };

        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type",
            CustomProperties = new List<DistinctionTypeCustomProperty>
            {
                existingDistinctionTypeCustomProperty
            }
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionTypeCustomProperty = new
        {
            existingDistinctionTypeCustomProperty.UniqueName
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameNull(string identity, string group, string member)
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = (string?) null
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var createdDistinctionTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .Single(x =>
                    x.Snowflake == existingDistinctionType.Snowflake &&
                    x.UniqueName == existingDistinctionType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == postDistinctionTypeCustomProperty.UniqueName);

            Assert.Null(createdDistinctionTypeCustomProperty);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameEmpty(string identity, string group, string member)
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = string.Empty
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var createdDistinctionTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .Single(x =>
                    x.Snowflake == existingDistinctionType.Snowflake &&
                    x.UniqueName == existingDistinctionType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == postDistinctionTypeCustomProperty.UniqueName);

            Assert.Null(createdDistinctionTypeCustomProperty);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameTooLong(string identity, string group, string member)
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray())
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var createdDistinctionTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .Single(x =>
                    x.Snowflake == existingDistinctionType.Snowflake &&
                    x.UniqueName == existingDistinctionType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == postDistinctionTypeCustomProperty.UniqueName);

            Assert.Null(createdDistinctionTypeCustomProperty);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameInvalid(string identity, string group, string member)
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            Snowflake = 1,
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postDistinctionTypeCustomProperty = new
        {
            UniqueName = "Invalid"
        };

        request.Content = JsonContent.Create(postDistinctionTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var createdDistinctionTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .Single(x =>
                    x.Snowflake == existingDistinctionType.Snowflake &&
                    x.UniqueName == existingDistinctionType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == postDistinctionTypeCustomProperty.UniqueName);

            Assert.Null(createdDistinctionTypeCustomProperty);
        }
    }
}