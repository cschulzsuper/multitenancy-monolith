﻿using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Extension.DistinctionTypeResource;

public sealed class Get 
{
    [Fact]
    public async Task Get_ShouldSucceed_WhenExists()
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

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("displayName", existingDistinctionType.DisplayName), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("objectType", existingDistinctionType.ObjectType), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", existingDistinctionType.UniqueName), (x.Key, (string?)x.Value)));
    }

    [Fact]
    public async Task Get_ShouldFail_WhenAbsent()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var absentDistinctionType = "absent-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/distinction-types/{absentDistinctionType}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Get_ShouldFail_WhenInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidDistinctionType = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/extension/distinction-types/{invalidDistinctionType}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}