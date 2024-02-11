using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Diagnostic;

internal static class BuildInfoResource
{
    private const string CouldNotQueryBuildInfo = "Could not query planned job";

    public static IEndpointRouteBuilder MapBuildInfoResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/build-info")
            .WithTags("Build Info API")
            .AllowAnonymous();

        resource
            .MapGet(string.Empty, Get)
            .WithErrorMessage(CouldNotQueryBuildInfo);

        return endpoints;
    }

    private static Delegate Get =>
        (IBuildInfoRequestHandler requestHandler)
            => requestHandler.Get();
}