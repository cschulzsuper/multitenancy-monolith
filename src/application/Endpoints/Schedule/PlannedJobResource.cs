using ChristianSchulz.MultitenancyMonolith.Application.Schedule.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

internal static class PlannedJobResource
{
    private const string CouldNotQueryPlannedJobs = "Could not query planned jobs";
    private const string CouldNotQueryPlannedJob = "Could not query planned job";
    private const string CouldNotUpdatePlannedJob = "Could not update planned job";

    public static IEndpointRouteBuilder MapPlannedJobResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/planned-jobs")
            .WithTags("Planned Job API")
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "identity")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotQueryPlannedJobs);

        resource
            .MapGet("{job}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
             .WithErrorMessage(CouldNotQueryPlannedJob);

        resource
            .MapPut("{job}", Update)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
             .WithErrorMessage(CouldNotUpdatePlannedJob);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IPlannedJobRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        (IPlannedJobRequestHandler requestHandler, string job)
            => requestHandler.GetAsync(job);

    private static Delegate Update =>
        (IPlannedJobRequestHandler requestHandler, string job, PlannedJobRequest request)
            => requestHandler.UpdateAsync(job, request);
}