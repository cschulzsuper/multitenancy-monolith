﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using ChristianSchulz.MultitenancyMonolith.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Administration.DistinctionTypeResource;

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Put_ShouldRespectMultitenancy_WhenSuccessful()
    {
        // Arrange
        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type"
        };

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/administration/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(MockWebApplication.Group1);

        var putDistinctionType = new
        {
            UniqueName = $"put-member-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Put Distinction Type"
        };

        request.Content = JsonContent.Create(putDistinctionType);

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
            Assert.Equal(existingDistinctionType.Snowflake, updatedDistinctionType.Snowflake);
            Assert.Equal(existingDistinctionType.UniqueName, updatedDistinctionType.UniqueName);
            Assert.Equal(existingDistinctionType.ObjectType, updatedDistinctionType.ObjectType);
            Assert.Equal(existingDistinctionType.DisplayName, updatedDistinctionType.DisplayName);
        }
    }
}