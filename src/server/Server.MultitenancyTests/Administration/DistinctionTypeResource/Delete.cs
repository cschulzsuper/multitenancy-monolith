﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Administration.DistinctionTypeResource;

public sealed class Delete : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Delete(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Delete_ShouldRespectMultitenancy_WhenSuccessful()
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group1))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(MockWebApplication.Group1);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            var updatedDistinctionType = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedDistinctionType);
        }
    }
}