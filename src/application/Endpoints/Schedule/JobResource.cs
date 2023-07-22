using ChristianSchulz.MultitenancyMonolith.Application.Schedule.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

internal static class JobResource
{
    private const string CouldNotQueryJobs = "Could not query jobs";
    private const string CouldNotQueryJob = "Could not query job";
    private const string CouldNotUpdateJob = "Could not update job";

    public static IEndpointRouteBuilder MapScheduleJobResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/jobs")
            .WithTags("Job API")
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "identity")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotQueryJobs);

        resource
            .MapGet("{job}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
             .WithErrorMessage(CouldNotQueryJob);

        resource
            .MapPut("{job}", Update)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
             .WithErrorMessage(CouldNotUpdateJob);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IJobRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        (IJobRequestHandler requestHandler, string job)
            => requestHandler.Get(job);

    private static Delegate Update =>
        (IJobRequestHandler requestHandler, string job, JobRequest request)
            => requestHandler.Update(job, request);
}