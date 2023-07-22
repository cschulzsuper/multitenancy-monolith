using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using ChristianSchulz.MultitenancyMonolith.Server;

namespace Schedule.JobResource;

// TODO Uncomment test elements when API is fully implemented

public sealed class Put : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Put(WebApplicationFactory<Program> factory)
    {
        _factory = factory.Mock();
    }

    [Fact]
    public async Task Put_ShouldSucceed_WhenValid()
    {
        // Arrange
        var existingJob = "mock-job";

#if false
        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IJobQueue>()
                .Enqueue("mock-job", new CronExpressionSchedule($"*/2 * * * *"), (ClaimsPrincipal _) => Task.CompletedTask);
        }
#endif

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/schedule/jobs/{existingJob}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putJob = new
        {
            Expression = $"*/1 * * * *"
        };

        request.Content = JsonContent.Create(putJob);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

#if false
        using (var scope = _factory.CreateMultitenancyScope())
        {
            var changedJob = scope.ServiceProvider
                .GetRequiredService<IJobQueue>()
                .PeekAll()
                .SingleOrDefault(x =>
                    x.UniqueName == existingJob &&
                    x.Timestamp == CronExpression.Parse(putJob.Expression).GetNextOccurrence(DateTime.Now));

            Assert.NotNull(changedJob);
        }
#endif
    }

#if false
    [Fact]
    public async Task Put_ShouldFail_WhenInvalid()
    {
        // Arrange
        var invalidJob = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/schedule/jobs/{invalidJob}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putJob = new
        {
            Expression = $"*/1 * * * *"
        };

        request.Content = JsonContent.Create(putJob);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAbsent()
    {
        // Arrange
        var absentJob = "absent-job";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/schedule/jobs/{absentJob}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putJob = new
        {
            Expression = $"*/1 * * * *"
        };

        request.Content = JsonContent.Create(putJob);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
#endif

    [Fact]
    public async Task Put_ShouldFail_WhenExpressionNull()
    {
        // Arrange
        var existingJob = "mock-job";

#if false
        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IJobQueue>()
                .Enqueue("mock-job", new CronExpressionSchedule($"*/2 * * * *"), (ClaimsPrincipal _) => Task.CompletedTask);
        }
#endif

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/schedule/jobs/{existingJob}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putJob = new
        {
            Expression = (string?)null
        };

        request.Content = JsonContent.Create(putJob);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenExpressionInvalid()
    {
        // Arrange
        var existingJob = "mock-job";

#if false
        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IJobQueue>()
                .Enqueue("mock-job", new CronExpressionSchedule($"*/2 * * * *"), (ClaimsPrincipal _) => Task.CompletedTask);
        }
#endif

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/schedule/jobs/{existingJob}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putJob = new
        {
            Expression = "Invalid"
        };

        request.Content = JsonContent.Create(putJob);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenExpressionEmpty()
    {
        // Arrange
        var existingJob = "mock-job";

#if false
        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IJobQueue>()
                .Enqueue("mock-job", new CronExpressionSchedule($"*/2 * * * *"), (ClaimsPrincipal _) => Task.CompletedTask);
        }
#endif

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/schedule/jobs/{existingJob}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putJob = new
        {
            Expression = string.Empty
        };

        request.Content = JsonContent.Create(putJob);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenExpressionTooLong()
    {
        // Arrange
        var existingJob = "mock-job";

#if false
        using (var scope = _factory.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IJobQueue>()
                .Enqueue("mock-job", new CronExpressionSchedule($"*/2 * * * *"), (ClaimsPrincipal _) => Task.CompletedTask);
        }
#endif

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/schedule/jobs/{existingJob}");
        request.Headers.Authorization = _factory.MockValidIdentityAuthorizationHeader();

        var putJob = new
        {
            Expression = new string(Enumerable.Repeat('a', 141).ToArray())
        };

        request.Content = JsonContent.Create(putJob);

        var client = _factory.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}