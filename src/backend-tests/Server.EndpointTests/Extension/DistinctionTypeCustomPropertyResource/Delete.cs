using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Extension.DistinctionTypeCustomPropertyResource;

public sealed class Delete 
{
    [Fact]
    public async Task Delete_ShouldSucceed_WhenExists()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingDistinctionTypeCustomProperty = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property--{Guid.NewGuid()}",
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

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{existingDistinctionTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var deletedDistinctionTypeCustomProperty = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .Single(x => x.UniqueName == existingDistinctionType.UniqueName)
                .CustomProperties
                .SingleOrDefault(x => x.UniqueName == existingDistinctionTypeCustomProperty.UniqueName);

            Assert.Null(deletedDistinctionTypeCustomProperty);
        }
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenInvalidDistinctionType()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidDistinctionType = "Invalid";
        var validDistinctionTypeCustomProperty = "valid-distinction-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/extension/distinction-types/{invalidDistinctionType}/custom-properties/{validDistinctionTypeCustomProperty}");
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

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{invalidDistinctionTypeCustomProperty}");
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

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{absentDistinctionTypeCustomProperty}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}