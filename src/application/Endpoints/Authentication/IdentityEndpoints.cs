using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var identitiesEndpoints = endpoints
            .MapGroup("/identities")
            .WithTags("Identities");

        identitiesEndpoints.MapGet(string.Empty, GetAll);
        identitiesEndpoints.MapGet("{identity}", Get);

        return endpoints;
    }

    private static Delegate GetAll =>
        [Authorize]
        (IIdentityRequestHandler requestHandler) 
            => requestHandler.GetAll();

    private static Delegate Get =>
        [Authorize]
        (IIdentityRequestHandler requestHandler, string identity)
            => requestHandler.Get(identity);
}
