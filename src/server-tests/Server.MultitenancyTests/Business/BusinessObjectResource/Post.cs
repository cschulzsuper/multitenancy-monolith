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

public sealed class Post : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Post(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Post_ShouldRespectMultitenancy()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/business/business-objects");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(MockWebApplication.Group1);

        var postBusinessObject = new
        {
            UniqueName = $"post-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(postBusinessObject);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group1))
        {
            var createdBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(createdBusinessObject);
        }

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            var createdBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.Null(createdBusinessObject);
        }
    }
}