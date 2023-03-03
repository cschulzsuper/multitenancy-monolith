using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Administration.DistinctionTypeCustomPropertyResource;

public sealed class GetAll : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetAll(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task GetAll_ShouldRespectMultitenancy()
    {
        // Arrange
        var existingDistinctionTypeCustomProperty = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property-{Guid.NewGuid()}",
        };

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group1))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        existingDistinctionType.CustomProperties.Add(existingDistinctionTypeCustomProperty);

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(MockWebApplication.Group1);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Empty(content.RootElement.EnumerateArray());
    }
}