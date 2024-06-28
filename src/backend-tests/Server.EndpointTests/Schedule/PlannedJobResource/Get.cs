using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidators;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Schedule.PlannedJobResource;

public sealed class Get 
{
    [Fact]
    public async Task Get_ShouldSucceed_WhenExists()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingPlannedJob = new PlannedJob
        {
            UniqueName = "mock-job-1",
            Expression = "*/1 * * * *",
            ExpressionType = ScheduleExpressionTypes.CronExpression
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<PlannedJob>>()
                .Insert(existingPlannedJob);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/schedule/planned-jobs/{existingPlannedJob.UniqueName}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(content);
        Assert.Collection(content.OrderBy(x => x.Key),
            x => Assert.Equal(("expression", existingPlannedJob.Expression), (x.Key, (string?)x.Value)),
            x => Assert.Equal(("uniqueName", existingPlannedJob.UniqueName), (x.Key, (string?)x.Value)));
    }

    [Fact]
    public async Task Get_ShouldFail_WhenAbsent()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var absentPlannedJob = "absent-planned-job";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/schedule/planned-jobs/{absentPlannedJob}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

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

        var invalidPlannedJob = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/a1/schedule/planned-jobs/{invalidPlannedJob}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
