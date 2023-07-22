using ChristianSchulz.MultitenancyMonolith.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using ChristianSchulz.MultitenancyMonolith.Server;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;

namespace Extension.ObjectTypeCustomPropertyResource;

public sealed class Post : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Post(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Post_ShouldSucceed_WhenObjectTypeNew()
    {
        // Arrange
        var existingObjectType = "business-object";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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
            x => Assert.Equal(("displayName", postObjectTypeCustomProperty.DisplayName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("objectType", existingObjectType), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("propertyName", postObjectTypeCustomProperty.PropertyName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("propertyType", postObjectTypeCustomProperty.PropertyType), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", postObjectTypeCustomProperty.UniqueName), (x.Key, (string?)x.Value)));

        using (var scope = _factory.CreateMultitenancyScope())
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

    [Fact]
    public async Task Post_ShouldSucceed_WhenPropertyTypeString()
    {
        // Arrange
        var existingObjectType = new ObjectType
        {
            UniqueName = "business-object"
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType.UniqueName}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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
            x => Assert.Equal(("displayName", postObjectTypeCustomProperty.DisplayName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("objectType", existingObjectType.UniqueName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("propertyName", postObjectTypeCustomProperty.PropertyName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("propertyType", postObjectTypeCustomProperty.PropertyType), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", postObjectTypeCustomProperty.UniqueName), (x.Key, (string?)x.Value)));

        using (var scope = _factory.CreateMultitenancyScope())
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

    [Fact]
    public async Task Post_ShouldFail_WhenInvalidObjectType()
    {
        // Arrange
        var invalidObjectType = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{invalidObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameExists()
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
            UniqueName = "business-object",
            CustomProperties = new List<ObjectTypeCustomProperty>
        {
            existingObjectTypeCustomProperty
        }
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType.UniqueName}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postObjectTypeCustomProperty = new
        {
            existingObjectTypeCustomProperty.UniqueName,
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
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

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        var existingObjectType = "business-object";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postObjectTypeCustomProperty = new
        {
            UniqueName = (string?)null,
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

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameEmpty()
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameTooLong()
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenUniqueNameInvalid()
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenDisplayNameNull()
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = (string?)null,
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

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenDisplayNameEmpty()
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenDisplayNameTooLong()
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenPropertyNameExists()
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
            UniqueName = "business-object",
            CustomProperties = new List<ObjectTypeCustomProperty>
        {
            existingObjectTypeCustomProperty
        }
        };

        using (var scope = _factory.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType.UniqueName}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postObjectTypeCustomProperty = new
        {
            UniqueName = $"post-object-type-custom-property-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            existingObjectTypeCustomProperty.PropertyName,
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using (var scope = _factory.CreateMultitenancyScope())
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

    [Fact]
    public async Task Post_ShouldFail_WhenPropertyNameNull()
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = (string?)null,
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenPropertyNameEmpty()
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenPropertyNameTooLong()
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenPropertyNameInvalid()
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenPropertyTypeNull()
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

        var postObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "postFooBar",
            PropertyType = (string?)null
        };

        request.Content = JsonContent.Create(postObjectTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenPropertyTypeEmpty()
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }

    [Fact]
    public async Task Post_ShouldFail_WhenPropertyTypeInvalid()
    {
        // Arrange
        var existingObjectType = "existing-object-type";

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/extension/object-types/{existingObjectType}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader();

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

        using var scope = _factory.CreateMultitenancyScope();

        var createdObjectTypeCustomProperty = scope.ServiceProvider
            .GetRequiredService<IRepository<ObjectType>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == postObjectTypeCustomProperty.UniqueName);

        Assert.Null(createdObjectTypeCustomProperty);
    }
}