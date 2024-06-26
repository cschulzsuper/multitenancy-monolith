﻿using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Extension.DistinctionTypeCustomPropertyResource;

public sealed class GetAll 
{
    [Fact]
    public async Task GetAll_ShouldSucceed()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingDistinctionTypeCustomProperty1 = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property-1-{Guid.NewGuid()}",
        };

        var existingDistinctionTypeCustomProperty2 = new DistinctionTypeCustomProperty
        {
            UniqueName = $"existing-distinction-type-custom-property-2-{Guid.NewGuid()}",
        };

        var existingDistinctionType = new DistinctionType
        {
            UniqueName = $"existing-distinction-type-{Guid.NewGuid()}",
            ObjectType = "business-object",
            DisplayName = "Existing Distinction Type",
            CustomProperties = new List<DistinctionTypeCustomProperty>
        {
            existingDistinctionTypeCustomProperty1, existingDistinctionTypeCustomProperty2
        }
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .Insert(existingDistinctionType);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}/custom-properties");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x =>
            {
                Assert.Equal(existingDistinctionTypeCustomProperty1.UniqueName, x.GetString("uniqueName"));
                Assert.Equal(existingDistinctionType.UniqueName, x.GetString("distinctionType"));
            },
            x =>
            {
                Assert.Equal(existingDistinctionTypeCustomProperty2.UniqueName, x.GetString("uniqueName"));
                Assert.Equal(existingDistinctionType.UniqueName, x.GetString("distinctionType"));
            });
    }

    [Fact]
    public async Task GetAll_ShouldFail_WhenInvalidDistinctionType()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidDistinctionType = "Invalid-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/distinction-types/{invalidDistinctionType}/custom-properties");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}