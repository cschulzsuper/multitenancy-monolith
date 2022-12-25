using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;
using Xunit;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Administration.MemberEndpoints;

public sealed class Post : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Post(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Guest)]
    public async Task Post_ShouldReturnCreatedMember_WhenValidMemberIsGiven(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var newMember = new
        {
            UniqueName = $"new-member-{Guid.NewGuid()}"
        };

        request.Content = JsonContent.Create(newMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadFromJsonAsync<JsonObject>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Collection(content,
            x => Assert.Equal((x.Key, (string?)x.Value), ("uniqueName", newMember.UniqueName)));
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Guest)]
    public async Task Post_ShouldCreateMemberInRepository_WhenValidMemberIsGiven(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var newMember = new
        {
            UniqueName = $"new-member-{Guid.NewGuid()}"
        };

        request.Content = JsonContent.Create(newMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        using var scope = _factory.Services.CreateMultitenancyScope(group);

        var createdMember = scope.ServiceProvider
            .GetRequiredService<IRepository<Member>>()
            .GetQueryable()
            .SingleOrDefault(x => x.UniqueName == newMember.UniqueName);

        Assert.NotNull(createdMember);
    }

    [Theory]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup1, TestConfiguration.DefaultGroup1Guest)]
    [InlineData(TestConfiguration.AdminIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Admin)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.DefaultGroup2, TestConfiguration.DefaultGroup2Guest)]
    public async Task Post_ShouldNotCreateMember_WhenMemberUniqueNameIsEmpty(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var newMember = new
        {
            UniqueName = string.Empty
        };

        request.Content = JsonContent.Create(newMember);

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
    public async Task Post_ShouldNotCreateMember_WhenMemberUniqueNameIsNull(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, $"/members");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var newMember = new
        {
            UniqueName = (string?)null
        };

        request.Content = JsonContent.Create(newMember);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
