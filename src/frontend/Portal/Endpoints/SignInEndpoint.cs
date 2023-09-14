using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Security;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Endpoints;

public static class SignInEndpoint
{
    public static IEndpointRouteBuilder MapSignIn(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("sign-in", (HttpContext context,
            [FromForm(Name = "return")] string @return,
            [FromForm(Name = "access-code")] string accessCode) =>
        {
            context.Response.StatusCode = 302;
            context.Response.Cookies.Append(BearerTokenConstants.CookieName, accessCode!,
                new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    HttpOnly = true
                });

            return Results.Redirect(@return!);
        })
            .AllowAnonymous()
            .DisableAntiforgery();


        return endpoints;
    }
}
