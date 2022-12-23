﻿using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
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
        identitiesEndpoints.MapPost(string.Empty, Post);
        identitiesEndpoints.MapDelete("{identity}", Delete);

        return endpoints;
    }

    private static Delegate GetAll =>
        [Authorize]
        (IIdentityRequestHandler requestHandler) 
            => requestHandler.GetAll();

    private static Delegate Get =>
        [Authorize]
        (IIdentityRequestHandler requestHandler, string identity)
            => requestHandler.GetAsync(identity);

    private static Delegate Post =>
        [Authorize]
        (IIdentityRequestHandler requestHandler, IdentityRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Delete =>
        [Authorize]
        (IIdentityRequestHandler requestHandler, string identity)
            => requestHandler.DeleteAsync(identity);
}
