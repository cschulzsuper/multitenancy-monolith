using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System;
using Microsoft.Extensions.DependencyInjection;
using ChristianSchulz.MultitenancyMonolith.Data;
using System.Linq;
using ChristianSchulz.MultitenancyMonolith.Server;
using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;

namespace Authorization.MemberResource;

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
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/authorization/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(MockWebApplication.Group1);

        var postMember = new
        {
            UniqueName = $"post-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(postMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group1))
        {
            var createdMember = scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(createdMember);
        }

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            var createdMember = scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.Null(createdMember);
        }
    }
}