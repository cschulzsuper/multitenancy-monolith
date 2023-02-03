using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.ObjectTypeCustomPropertyResource;

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
        var validObjectType = "required-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{validObjectType}/custom-properties");

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

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
        var validObjectType = "required-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{validObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader(identity);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

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
        var validObjectType = "required-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{validObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

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
    public async Task Post_ShouldSucceed_WhenObjectTypeNew(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "business-object";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = $"post-object-type-custom-property-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("displayName", postObjectTypeCustomProperty.DisplayName), (x.Key, (string?) x.Value)),
            x => Assert.Equal(("objectType", existingObjectType), (x.Key, (string?) x.Value)),
            x => Assert.Equal(("propertyName", postObjectTypeCustomProperty.PropertyName), (x.Key, (string?) x.Value)),
            x => Assert.Equal(("propertyType", postObjectTypeCustomProperty.PropertyType), (x.Key, (string?) x.Value)),
            x => Assert.Equal(("uniqueName", postObjectTypeCustomProperty.UniqueName), (x.Key, (string?) x.Value)));

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var createdObjectTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .GetQueryable()
                .Single(x => x.UniqueName == existingObjectType)
                .CustomProperties
                .SingleOrDefault(x =>
                    x.UniqueName == postObjectTypeCustomProperty.UniqueName &&
                    x.DisplayName == postObjectTypeCustomProperty.DisplayName &&
                    x.PropertyName == postObjectTypeCustomProperty.PropertyName &&
                    x.PropertyType == postObjectTypeCustomProperty.PropertyType);

            Assert.NotNull(createdObjectTypeCustomProperty);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldSucceed_WhenPropertyTypeString(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = new ObjectType
        {
            Snowflake = 1,
            UniqueName = "business-object"
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType.UniqueName}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = $"post-object-type-custom-property-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("displayName", postObjectTypeCustomProperty.DisplayName), (x.Key, (string?) x.Value)),
            x => Assert.Equal(("objectType", existingObjectType.UniqueName), (x.Key, (string?) x.Value)),
            x => Assert.Equal(("propertyName", postObjectTypeCustomProperty.PropertyName), (x.Key, (string?) x.Value)),
            x => Assert.Equal(("propertyType", postObjectTypeCustomProperty.PropertyType), (x.Key, (string?) x.Value)),
            x => Assert.Equal(("uniqueName", postObjectTypeCustomProperty.UniqueName), (x.Key, (string?) x.Value)));

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var createdObjectTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .GetQueryable()
                .Single(x => x.UniqueName == existingObjectType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x =>
                    x.UniqueName == postObjectTypeCustomProperty.UniqueName &&
                    x.DisplayName == postObjectTypeCustomProperty.DisplayName &&
                    x.PropertyName == postObjectTypeCustomProperty.PropertyName &&
                    x.PropertyType == postObjectTypeCustomProperty.PropertyType);

            Assert.NotNull(createdObjectTypeCustomProperty);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenInvalidObjectType(string identity, string group, string member)
    {
        // Arrange
        var invalidObjectType = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{invalidObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

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
        var existingObjectTypeCustomProperty = new ObjectTypeCustomProperty
        {
            UniqueName = $"existing-object-type-custom-property--{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        var existingObjectType = new ObjectType
        {
            Snowflake = 1,
            UniqueName = "business-object",
            CustomProperties = new List<ObjectTypeCustomProperty>
            {
                existingObjectTypeCustomProperty
            }
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType.UniqueName}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = existingObjectTypeCustomProperty.UniqueName,
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var unchangedObjectTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .GetQueryable()
                .Single(x => x.UniqueName == existingObjectType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x =>
                    x.UniqueName == existingObjectTypeCustomProperty.UniqueName &&
                    x.DisplayName == postObjectTypeCustomProperty.DisplayName &&
                    x.PropertyName == postObjectTypeCustomProperty.PropertyName &&
                    x.PropertyType == postObjectTypeCustomProperty.PropertyType);

            Assert.NotNull(unchangedObjectTypeCustomProperty);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameNull(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "business-object";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = (string?) null,
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameEmpty(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = string.Empty,
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameTooLong(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenUniqueNameInvalid(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "Invalid",
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenDisplayNameNull(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = (string?) null,
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenDisplayNameEmpty(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = string.Empty,
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenDisplayNameTooLong(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = new string(Enumerable.Repeat('a', 141).ToArray()),
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenPropertyNameExists(string identity, string group, string member)
    {
        // Arrange
        var existingObjectTypeCustomProperty = new ObjectTypeCustomProperty
        {
            UniqueName = $"existing-object-type-custom-property-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "existingFooBar",
            PropertyType = "string"
        };

        var existingObjectType = new ObjectType
        {
            Snowflake = 1,
            UniqueName = "business-object",
            CustomProperties = new List<ObjectTypeCustomProperty>
            {
                existingObjectTypeCustomProperty
            }
        };

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType.UniqueName}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = $"post-object-type-custom-property-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = existingObjectTypeCustomProperty.PropertyName,
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var unchangedObjectTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .GetQueryable()
                .Single(x => x.UniqueName == existingObjectType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x =>
                    x.UniqueName == existingObjectTypeCustomProperty.UniqueName &&
                    x.DisplayName == postObjectTypeCustomProperty.DisplayName &&
                    x.PropertyName == postObjectTypeCustomProperty.PropertyName &&
                    x.PropertyType == postObjectTypeCustomProperty.PropertyType);

            Assert.NotNull(unchangedObjectTypeCustomProperty);
        }
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenPropertyNameNull(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = (string?) null,
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenPropertyNameEmpty(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = string.Empty,
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenPropertyNameTooLong(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = new string(Enumerable.Repeat('a', 141).ToArray()),
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenPropertyNameInvalid(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "Invalid",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenPropertyTypeNull(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = (string?) null
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenPropertyTypeEmpty(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = string.Empty
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    public async Task Post_ShouldFail_WhenPropertyTypeInvalid(string identity, string group, string member)
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/administration/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "Invalid"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }
}