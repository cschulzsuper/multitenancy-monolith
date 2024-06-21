using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Business.BusinessObjectResource;

public sealed class Delete 
{
    [Fact]
    public async Task Delete_ShouldSucceed_WhenExists()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingBusinessObject = new BusinessObject
        {
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var deletedBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault(x => x.UniqueName == existingBusinessObject.UniqueName);

            Assert.Null(deletedBusinessObject);
        }
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenAbsent()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var absentBusinessObject = "absent-business-object";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/business/business-objects/{absentBusinessObject}");
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

        var invalidBusinessObject = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/a1/business/business-objects/{invalidBusinessObject}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}