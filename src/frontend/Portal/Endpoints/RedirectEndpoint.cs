using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Endpoints;

public static class RedirectEndpoint
{
    public static IEndpointRouteBuilder MapRedirect(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("redirect", (HttpContext context) =>
        {
            var @return = context.Request.Query["return"];
            if (string.IsNullOrWhiteSpace(@return))
            {
                return Results.NotFound();
            }

            context.Request.Cookies.TryGetValue("access-token", out var accessToken);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return Results.NotFound();
            }

            var returnUri = new Uri(@return!);
            var returnUriScheme = returnUri.Scheme;
            var returnUriHost = returnUri.Host;
            var returnUriPort = returnUri.Port;
            var returnUriSegment = returnUri.Segments.Skip(1).Select(x => x.Trim('/') + '/').FirstOrDefault() ?? string.Empty;

            var redirect = $"{returnUriScheme}://{returnUriHost}:{returnUriPort}/{returnUriSegment}sign-in";

            var redirectContent =
                $"""
                    <html>
                        <head>
                            <title>Auth Redirect</title>
                        </head>
                        <body onload="document.forms[0].submit()">
                            <form method="POST" action="{redirect}">;
                                <input type="hidden" name="access-code" value="{accessToken}" />
                                <input type="hidden" name="return" value="{@return}" />
                                <noscript>
                                    <p>JavaScript is disabled. Click the button below to continue.</p>
                                    <input name="continue" type="submit" value="CONTINUE" />
                                </noscript>
                            </form>
                        </body>
                    </html>
                """;

            return Results.Content(redirectContent, "text/html");
        });

        return endpoints;
    }
}
