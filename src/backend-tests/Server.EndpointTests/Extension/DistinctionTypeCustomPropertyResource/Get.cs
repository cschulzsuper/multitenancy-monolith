using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Extension.DistinctionTypeCustomPropertyResource;

public sealed class Get 
{
    [Fact]
    public async Task Get_ShouldSucceed_WhenExists()
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

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{existingDistinctionTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("distinctionType", existingDistinctionType.UniqueName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", existingDistinctionTypeCustomProperty.UniqueName), (x.Key, (string?)x.Value)));
    }

    [Fact]
    public async Task Get_ShouldFail_WhenInvalidDistinctionType()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidDistinctionType = "Invalid-distinction-type";
        var validDistinctionTypeCustomProperty = "valid-distinction-type-custom-property";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/distinction-types/{invalidDistinctionType}/custom-properties/{validDistinctionTypeCustomProperty}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Get_ShouldFail_WhenInvalidCustomProperty()
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

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{invalidDistinctionTypeCustomProperty}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Get_ShouldFail_WhenAbsent()
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

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{absentDistinctionTypeCustomProperty}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}