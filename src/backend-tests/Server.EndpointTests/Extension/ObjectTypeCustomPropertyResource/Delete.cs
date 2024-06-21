using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Extension.ObjectTypeCustomPropertyResource;

public sealed class Delete 
{

    [Fact]
    public async Task Delete_ShouldSucceed_WhenExists()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingObjectTypeCustomProperty = new ObjectTypeCustomProperty
        {
            UniqueName = "business-object",
            DisplayName = "Foo Bar",
            PropertyName = "fooBar",
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

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{existingObjectTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var deletedObjectType = scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .GetQueryable()
                .Single(x => x.UniqueName == existingObjectType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == existingObjectTypeCustomProperty.UniqueName);

            Assert.Null(deletedObjectType);
        }
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenInvalidObjectType()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidObjectType = "Invalid";
        var validObjectTypeCustomProperty = "valid-object-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/extension/object-types/{invalidObjectType}/custom-properties/{validObjectTypeCustomProperty}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenInvalidCustomProperty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingObjectType = new ObjectType
        {
            UniqueName = "business-object"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var invalidObjectTypeCustomProperty = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{invalidObjectTypeCustomProperty}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenAbsent()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingObjectType = new ObjectType
        {
            UniqueName = "business-object"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);
        }

        var absentObjectTypeCustomProperty = "absent-object-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/extension/object-types/{existingObjectType.UniqueName}/custom-properties/{absentObjectTypeCustomProperty}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}