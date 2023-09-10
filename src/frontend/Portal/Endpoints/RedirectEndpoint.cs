using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
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

            context.Request.Cookies.TryGetValue("access_token", out var accessToken);
            if (string.IsNullOrWhiteSpace(accessToken)) 
            {
                return Results.Redirect($"/sign-in{context.Request.QueryString}");
            }
            else
            {
                var queryParameter = new[]
                {
                    new KeyValuePair<string, string?>("access_code", accessToken),
                    new KeyValuePair<string, string?>("return", @return)
                };

                var returnUri = new Uri(@return!);
                var returnUriScheme = returnUri.Scheme;
                var returnUriHost = returnUri.Host;
                var returnUriPort = returnUri.Port;
                var returnUriSegment = returnUri.Segments.Skip(1).Select(x => x.Trim('/') + '/').FirstOrDefault() ?? string.Empty;

                var redirect = $"{returnUriScheme}://{returnUriHost}:{returnUriPort}/{returnUriSegment}sign-in{QueryString.Create(queryParameter)}";

                return Results.Redirect(redirect);
            }
        });

        return endpoints;
    }
}
