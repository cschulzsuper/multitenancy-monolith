using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Swagger.Endpoints;

public static class SignInEndpoint
{
    public static IEndpointRouteBuilder MapSignIn(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("swagger/sign-in", (HttpContext context) =>
        {
            var @return = context.Request.Query["return"];
            if (string.IsNullOrWhiteSpace(@return))
            {
                return Results.NotFound();
            }

            context.Request.Query.TryGetValue("access_code", out var accessToken);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return Results.NotFound();
            }

            context.Response.StatusCode = 302;
            context.Response.Cookies.Append("access_token", accessToken!);

            return Results.Redirect(@return!);
        }).AllowAnonymous();

        return endpoints;
    }
}
