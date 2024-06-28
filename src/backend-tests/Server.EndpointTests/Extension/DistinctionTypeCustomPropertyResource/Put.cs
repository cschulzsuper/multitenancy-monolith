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

namespace Extension.DistinctionTypeCustomPropertyResource;

public sealed class Put 
{
    [Fact]
    public async Task Put_ShouldSucceed_WhenValid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingDistinctionTypeCustomProperty = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property-{Guid.NewGuid()}"
        };

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type",
            CustomProperties = new List<DistinctionTypeCustomProperty>
        {
            existingDistinctionTypeCustomProperty
        }
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{existingDistinctionTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionTypeCustomProperty = new
        {
            UniqueName = $"put-distinction-type-custom-property-{Guid.NewGuid()}"
        };

        request.Content = JsonContent.Create(putDistinctionTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var changedDistinctionTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .Single(x =>
                    x.Snowflake == existingDistinctionType.Snowflake &&
                    x.UniqueName == existingDistinctionType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == putDistinctionTypeCustomProperty.UniqueName);

            Assert.NotNull(changedDistinctionTypeCustomProperty);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalidDistinctionType()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidDistinctionType = "Invalid";
        var validDistinctionTypeCustomProperty = "valid-distinction-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{invalidDistinctionType}/custom-properties/{validDistinctionTypeCustomProperty}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionTypeCustomProperty = new
        {
            UniqueName = "put-distinction-type-custom-property"
        };

        request.Content = JsonContent.Create(putDistinctionTypeCustomProperty);

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

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var invalidDistinctionTypeCustomProperty = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{invalidDistinctionTypeCustomProperty}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionTypeCustomProperty = new
        {
            UniqueName = "put-distinction-type-custom-property"
        };

        request.Content = JsonContent.Create(putDistinctionTypeCustomProperty);

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

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var absentDistinctionTypeCustomProperty = "absent-distinction-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{absentDistinctionTypeCustomProperty}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionTypeCustomProperty = new
        {
            UniqueName = "put-distinction-type-custom-property"
        };

        request.Content = JsonContent.Create(putDistinctionTypeCustomProperty);

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

        var existingDistinctionTypeCustomProperty = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property-{Guid.NewGuid()}"
        };

        var additionalDistinctionTypeCustomProperty = new DistinctionTypeCustomProperty
        {
            UniqueName = $"additional-distinction-type-custom-property-{Guid.NewGuid()}"
        };

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type",
            CustomProperties = new List<DistinctionTypeCustomProperty>()
        {
            existingDistinctionTypeCustomProperty, additionalDistinctionTypeCustomProperty
        }
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{existingDistinctionTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionTypeCustomProperty = new
        {
            additionalDistinctionTypeCustomProperty.UniqueName
        };

        request.Content = JsonContent.Create(putDistinctionTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var unchangedDistinctionTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .Single(x => x.UniqueName == existingDistinctionType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == existingDistinctionTypeCustomProperty.UniqueName);

            Assert.NotNull(unchangedDistinctionTypeCustomProperty);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingDistinctionTypeCustomProperty = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property-{Guid.NewGuid()}"
        };

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type",
            CustomProperties = new List<DistinctionTypeCustomProperty>()
        {
            existingDistinctionTypeCustomProperty
        }
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{existingDistinctionTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionTypeCustomProperty = new
        {
            UniqueName = (string?)null
        };

        request.Content = JsonContent.Create(putDistinctionTypeCustomProperty);

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

        var existingDistinctionTypeCustomProperty = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property-{Guid.NewGuid()}"
        };

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type",
            CustomProperties = new List<DistinctionTypeCustomProperty>()
        {
            existingDistinctionTypeCustomProperty
        }
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{existingDistinctionTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionTypeCustomProperty = new
        {
            UniqueName = string.Empty
        };

        request.Content = JsonContent.Create(putDistinctionTypeCustomProperty);

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

        var existingDistinctionTypeCustomProperty = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property-{Guid.NewGuid()}"
        };

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type",
            CustomProperties = new List<DistinctionTypeCustomProperty>()
        {
            existingDistinctionTypeCustomProperty
        }
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{existingDistinctionTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionTypeCustomProperty = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray())
        };

        request.Content = JsonContent.Create(putDistinctionTypeCustomProperty);

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

        var existingDistinctionTypeCustomProperty = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property-{Guid.NewGuid()}"
        };

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type",
            CustomProperties = new List<DistinctionTypeCustomProperty>()
        {
            existingDistinctionTypeCustomProperty
        }
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{existingDistinctionTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putDistinctionTypeCustomProperty = new
        {
            UniqueName = "Invalid"
        };

        request.Content = JsonContent.Create(putDistinctionTypeCustomProperty);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}