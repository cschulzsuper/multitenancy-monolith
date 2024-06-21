using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidators;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Schedule.PlannedJobResource;

public sealed class Put 
{
    [Fact]
    public async Task Put_ShouldSucceed_WhenValid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingPlannedJob = new PlannedJob
        {
            UniqueName = "mock-job-1",
            Expression = "*/5 * * * *",
            ExpressionType = ScheduleExpressionTypes.CronExpression
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<PlannedJob>>()
                .Insert(existingPlannedJob);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/schedule/planned-jobs/{existingPlannedJob.UniqueName}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var putPlannedJob = new
        {
            Expression = "*/1 * * * *"
        };

        request.Content = JsonContent.Create(putPlannedJob);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.Services.CreateScope())
        {
            var changedJob = scope.ServiceProvider
                .GetRequiredService<IRepository<PlannedJob>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.ExpressionType == existingPlannedJob.ExpressionType &&
                    x.Expression == "*/1 * * * *");

            Assert.NotNull(changedJob);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidPlannedJob = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/schedule/planned-jobs/{invalidPlannedJob}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var putPlannedJob = new
        {
            Expression = "*/1 * * * *"
        };

        request.Content = JsonContent.Create(putPlannedJob);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAbsent()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var absentPlannedJob = "absent-job";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/schedule/planned-jobs/{absentPlannedJob}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var putPlannedJob = new
        {
            Expression = "*/1 * * * *"
        };

        request.Content = JsonContent.Create(putPlannedJob);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenExpressionNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingPlannedJob = new PlannedJob
        {
            UniqueName = "mock-job-2",
            Expression = "*/5 * * * *",
            ExpressionType = ScheduleExpressionTypes.CronExpression
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<PlannedJob>>()
                .Insert(existingPlannedJob);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/schedule/planned-jobs/{existingPlannedJob.UniqueName}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var putPlannedJob = new
        {
            Expression = (string?)null
        };

        request.Content = JsonContent.Create(putPlannedJob);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenExpressionInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingPlannedJob = new PlannedJob
        {
            UniqueName = "mock-job-3",
            Expression = "*/5 * * * *",
            ExpressionType = ScheduleExpressionTypes.CronExpression
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<PlannedJob>>()
                .Insert(existingPlannedJob);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/schedule/planned-jobs/{existingPlannedJob.UniqueName}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var putPlannedJob = new
        {
            Expression = "Invalid"
        };

        request.Content = JsonContent.Create(putPlannedJob);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenExpressionEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingPlannedJob = new PlannedJob
        {
            UniqueName = "mock-job-4",
            Expression = "*/5 * * * *",
            ExpressionType = ScheduleExpressionTypes.CronExpression
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<PlannedJob>>()
                .Insert(existingPlannedJob);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/schedule/planned-jobs/{existingPlannedJob.UniqueName}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var putPlannedJob = new
        {
            Expression = string.Empty
        };

        request.Content = JsonContent.Create(putPlannedJob);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenExpressionTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingPlannedJob = new PlannedJob
        {
            UniqueName = "mock-job-5",
            Expression = "*/5 * * * *",
            ExpressionType = ScheduleExpressionTypes.CronExpression
        };

        using (var scope = application.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<PlannedJob>>()
                .Insert(existingPlannedJob);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/schedule/planned-jobs/{existingPlannedJob.UniqueName}");
        request.Headers.Authorization = application.MockValidIdentityAuthorizationHeader();

        var putPlannedJob = new
        {
            Expression = new string(Enumerable.Repeat('a', 4001).ToArray())
        };

        request.Content = JsonContent.Create(putPlannedJob);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}