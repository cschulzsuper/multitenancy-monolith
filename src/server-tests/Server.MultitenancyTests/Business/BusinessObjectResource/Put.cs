using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System;
using Microsoft.Extensions.DependencyInjection;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using System.Linq;
using ChristianSchulz.MultitenancyMonolith.Server;
using System.Collections.Generic;

namespace Business.BusinessObjectResource;

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
        var existingBusinessObject = new BusinessObject
        {
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(MockWebApplication.Group1);

        var putBusinessObject = new
        {
            UniqueName = $"put-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            var updatedBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedBusinessObject);
            Assert.Equal(existingBusinessObject.Snowflake, updatedBusinessObject.Snowflake);
            Assert.Equal(existingBusinessObject.UniqueName, updatedBusinessObject.UniqueName);
        }
    }
}