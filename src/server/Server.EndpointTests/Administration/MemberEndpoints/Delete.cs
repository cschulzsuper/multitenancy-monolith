using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;
using Xunit;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.MemberEndpoints;

public sealed class Delete : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Delete(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Guest)]
    public async Task Delete_ShouldDeleteMember_WhenExistingMemberIsGiven(string identity, string group, string member)
    {
        // Arrange
        var existingMember = $"existing-member-{Guid.NewGuid()}";

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(new Member
                {
                    Snowflake = 1,
                    UniqueName = existingMember,
                });
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/members/{existingMember}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Guest)]
    public async Task Delete_ShouldDeleteMemberInRepository_WhenExistingMemberIsGiven(string identity, string group, string member)
    {
        // Arrange
        var existingMember = $"existing-member-{Guid.NewGuid()}";

        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(new Member 
                {
                    Snowflake = 1,
                    UniqueName = existingMember,
                });
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/members/{existingMember}");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        using (var scope = _factory.Services.CreateMultitenancyScope(group))
        {
            var deletedMember = scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .GetQueryable()
                .SingleOrDefault(x => x.UniqueName == existingMember);

            Assert.Null(deletedMember);
        }
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Guest)]
    public async Task Delete_ShouldNotDeleteMember_WhenMemberDoesNotExist(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/members/foo-bar");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Guest)]
    public async Task Delete_ShouldNotDeleteMember_WhenMemberUniqueNameIsInvalid(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/members/FOOBAR");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
