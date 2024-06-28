using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Extension.ObjectTypeCustomPropertyResource;

public sealed class Put 
{
    [Fact]
    public async Task Put_ShouldSucceed_WhenPropertyTypeString()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingObjectTypeCustomProperty = new ObjectTypeCustomProperty
        {
            UniqueName = $"existing-object-type-custom-property--{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = $"put-object-type-custom-property-{Guid.NewGuid()}",
            DisplayName = "Put Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var changedObjectTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .GetQueryable()
                .Single(x => x.UniqueName == existingObjectType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x =>
                    x.UniqueName == putObjectTypeCustomProperty.UniqueName &&
                    x.PropertyName == putObjectTypeCustomProperty.PropertyName &&
                    x.PropertyType == putObjectTypeCustomProperty.PropertyType &&
                    x.DisplayName == putObjectTypeCustomProperty.DisplayName);

            Assert.NotNull(changedObjectTypeCustomProperty);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalidObjectType()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidObjectType = "Invalid";
        var validObjectTypeCustomProperty = "valid-object-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{invalidObjectType}/custom-properties/{validObjectTypeCustomProperty}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "put-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalidCustomProperty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingObjectType = new ObjectType
        {
            UniqueName = "business-object",
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var invalidObjectTypeCustomProperty = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{invalidObjectTypeCustomProperty}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "put-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

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
        using var application = MockWebApplication.Create();

        var existingObjectType = new ObjectType
        {
            UniqueName = "business-object",
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var absentObjectTypeCustomProperty = "absent-object-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{absentObjectTypeCustomProperty}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "put-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

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
        using var application = MockWebApplication.Create();

        var existingObjectTypeCustomProperty = new ObjectTypeCustomProperty
        {
            UniqueName = $"existing-object-type-custom-property-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "existingFooBar",
            PropertyType = "string"
        };

        var additionalObjectTypeCustomProperty = new ObjectTypeCustomProperty
        {
            UniqueName = $"additional-object-type-custom-property-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "additionalFooBar",
            PropertyType = "string"
        };

        var existingObjectType = new ObjectType
        {
            UniqueName = "business-object",
            CustomProperties = new List<ObjectTypeCustomProperty>
        {
            existingObjectTypeCustomProperty, additionalObjectTypeCustomProperty
        }
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            additionalObjectTypeCustomProperty.UniqueName,
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var unchangedObjectTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .GetQueryable()
                .Single(x => x.UniqueName == existingObjectType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x =>
                    x.UniqueName == existingObjectTypeCustomProperty.UniqueName &&
                    x.PropertyName == existingObjectTypeCustomProperty.PropertyName &&
                    x.PropertyType == existingObjectTypeCustomProperty.PropertyType &&
                    x.DisplayName == existingObjectTypeCustomProperty.DisplayName);

            Assert.NotNull(unchangedObjectTypeCustomProperty);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = (string?)null,
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

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
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = string.Empty,
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

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
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

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
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "Invalid",
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenDisplayNameNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = (string?)null,
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenDisplayNameEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = string.Empty,
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenDisplayNameTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = new string(Enumerable.Repeat('a', 141).ToArray()),
            PropertyName = "putFooBar",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenPropertyNameNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = (string?)null,
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenPropertyNameEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = string.Empty,
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenPropertyNameTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = new string(Enumerable.Repeat('a', 141).ToArray()),
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenPropertyNameInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "Invalid",
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenPropertyNameExists()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingObjectTypeCustomProperty = new ObjectTypeCustomProperty
        {
            UniqueName = $"existing-object-type-custom-property-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "existingFooBar",
            PropertyType = "string"
        };

        var additionalObjectTypeCustomProperty = new ObjectTypeCustomProperty
        {
            UniqueName = $"additional-object-type-custom-property-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            PropertyName = "additionalFooBar",
            PropertyType = "string"
        };

        var existingObjectType = new ObjectType
        {
            UniqueName = "business-object",
            CustomProperties = new List<ObjectTypeCustomProperty>
        {
            existingObjectTypeCustomProperty, additionalObjectTypeCustomProperty
        }
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = $"put-object-type-custom-property-{Guid.NewGuid()}",
            DisplayName = "Foo Bar",
            additionalObjectTypeCustomProperty.PropertyName,
            PropertyType = "string"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var unchangedObjectTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .GetQueryable()
                .Single(x => x.UniqueName == existingObjectType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x =>
                    x.UniqueName == existingObjectTypeCustomProperty.UniqueName &&
                    x.PropertyName == existingObjectTypeCustomProperty.PropertyName &&
                    x.PropertyType == existingObjectTypeCustomProperty.PropertyType &&
                    x.DisplayName == existingObjectTypeCustomProperty.DisplayName);

            Assert.NotNull(unchangedObjectTypeCustomProperty);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenPropertyTypeNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = (string?)null
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenPropertyTypeEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = string.Empty
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenPropertyTypeInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

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

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putObjectTypeCustomProperty = new
        {
            UniqueName = "post-object-type-custom-property",
            DisplayName = "Foo Bar",
            PropertyName = "putFooBar",
            PropertyType = "Invalid"
        };

        request.Content = JsonContent.Create(putObjectTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}