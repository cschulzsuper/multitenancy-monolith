using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace ChristianSchulz.MultitenancyMonolith.Server.EndpointTests.Authorization.AggregateTypeResource;

public sealed class GetAll : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetAll(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithInMemoryData();
    }

    [Theory]
    [Trait("Category", "Endpoint")]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group1, TestConfiguration.Group1Chief)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group1, TestConfiguration.Group1Member)]
    [InlineData(TestConfiguration.ChiefIdentity, TestConfiguration.Group2, TestConfiguration.Group2Chief)]
    [InlineData(TestConfiguration.GuestIdentity, TestConfiguration.Group2, TestConfiguration.Group2Member)]
    public async Task GetAll_ShouldReturnAggregateTypes(string identity, string group, string member)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/administration/aggregate-types");
        request.Headers.Authorization = _factory.MockValidMemberAuthorizationHeader(identity, group, member);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(content);
        Assert.Collection(content.RootElement.EnumerateArray().OrderBy(x => x.GetString("uniqueName")),
            x => 
            {
                Assert.Equal("business-object", x.GetString("uniqueName"));
                Assert.Equal("Business Object", x.GetString("displayName"));
                Assert.Equal("business", x.GetString("area"));
                Assert.Equal("business-objects", x.GetString("resource"));
            });
    }
}