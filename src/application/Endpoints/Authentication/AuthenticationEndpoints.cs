using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Request;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authenticationEndpoints = endpoints
            .MapGroup("/identities")
            .WithTags("Identities");

        authenticationEndpoints
            .MapPost("/{uniqueName}/sign-in", SignIn);

        return endpoints;
    }

    private static Delegate SignIn =>
        (IAuthenticationRequestHandler requestHandler, string uniqueName, SignInRequest request)
            => requestHandler.SignIn(uniqueName, request);
}
