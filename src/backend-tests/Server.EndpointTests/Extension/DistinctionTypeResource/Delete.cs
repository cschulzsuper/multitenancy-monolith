using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Extension.DistinctionTypeResource;

public sealed class Delete 
{
    [Fact]
    public async Task Delete_ShouldSucceed_WhenExists()
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

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/extension/distinction-types/{existingDistinctionType.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var deletedDistinctionType = scope.ServiceProvider
                .GetRequiredService<IRepository<DistinctionType>>()
                .GetQueryable()
                .SingleOrDefault(x => x.UniqueName == existingDistinctionType.UniqueName);

            Assert.Null(deletedDistinctionType);
        }
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenAbsent()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var absentDistinctionType = "absent-distinction-type";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/extension/distinction-types/{absentDistinctionType}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidDistinctionType = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/extension/distinction-types/{invalidDistinctionType}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}