using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
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

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Put_ShouldRespectMultitenancy()
    {
        // Arrange
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

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties/{existingDistinctionTypeCustomProperty.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(MockWebApplication.Group1);

        var putDistinctionTypeCustomProperty = new
        {
            UniqueName = $"put-distinction-type-custom-property-{Guid.NewGuid()}"
        };

        request.Content = JsonContent.Create(putDistinctionTypeCustomProperty);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            var updatedDistinctionType = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedDistinctionType);
            Assert.Collection(existingDistinctionType.CustomProperties,
                x => Assert.NotEqual(existingDistinctionTypeCustomProperty.UniqueName, putDistinctionTypeCustomProperty.UniqueName));
        }
    }
}