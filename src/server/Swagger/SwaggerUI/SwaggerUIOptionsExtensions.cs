using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger.SwaggerUI;

public static class SwaggerUIOptionsExtensions
{
    private const string AccessTokenRequestInterceptorJavaScript =
        """
            (req) => 
            {
                var swaggerJsonRequest = req.url.endsWith('swagger.json');
                if (!swaggerJsonRequest) 
                {
                    return req;
                }

                var queryString = new URLSearchParams(window.location.search);
                var accessToken = queryString.get('access_token');
            
                if(accessToken != null) 
                {
                    req.headers['Authorization'] = accessToken; 
                }

                return req; 
            }
        """;

    public static SwaggerUIOptions UseAccessTokenRequestInterceptor(this SwaggerUIOptions options)
    {
        var accessTokenRequestInterceptorJavaScript = AccessTokenRequestInterceptorJavaScript
            .Replace(Environment.NewLine, string.Empty);

        options.UseRequestInterceptor(accessTokenRequestInterceptorJavaScript);

        return options;
    }

    public static SwaggerUIOptions ConfigureSwaggerEndpoints(this SwaggerUIOptions options, ICollection<SwaggerDocs> swaggerDocs)
    {
        foreach (var swaggerDoc in swaggerDocs)
        {
            var hostUri = new Uri(swaggerDoc.Host);
            var fullUri = new Uri(hostUri, swaggerDoc.Path);

            options.SwaggerEndpoint(fullUri.AbsoluteUri, swaggerDoc.DisplayName);
        }

        return options;
    }
}