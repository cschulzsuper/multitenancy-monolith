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
        var existingMember = new Member
        {
            UniqueName = $"existing-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(existingMember);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/authorization/members/{existingMember.UniqueName}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(MockWebApplication.Group1);

        var putMember = new
        {
            UniqueName = $"put-member-{Guid.NewGuid()}",
            MailAddress = "default@localhost"
        };

        request.Content = JsonContent.Create(putMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using (var scope = _factory.CreateMultitenancyScope(MockWebApplication.Group2))
        {
            var updatedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .GetQueryable()
                .SingleOrDefault();

            Assert.NotNull(updatedMember);
            Assert.Equal(existingMember.Snowflake, updatedMember.Snowflake);
            Assert.Equal(existingMember.UniqueName, updatedMember.UniqueName);
            Assert.Equal(existingMember.MailAddress, updatedMember.MailAddress);
        }
    }
}