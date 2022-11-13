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
            .MapGroup("/users")
            .WithTags("User");

        authenticationEndpoints
            .MapPost("/{username}/sign-in", SignIn);

        return endpoints;
    }

    private static Delegate SignIn =>
        (IAuthenticationRequestHandler requestHandler, string username, SignInRequest request)
            => requestHandler.SignIn(username, request);
}
