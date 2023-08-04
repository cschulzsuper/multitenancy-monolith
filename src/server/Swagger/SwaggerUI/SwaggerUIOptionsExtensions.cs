using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.AspNetCore.Builder;
using NUglify;
using NUglify.JavaScript;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger.SwaggerUI;

public static class SwaggerUIOptionsExtensions
{
    private const string AccessTokenRequestInterceptorJavaScript =
        """
            function n(req)
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
        var minifiedRequestInterceptor = Uglify.Js(AccessTokenRequestInterceptorJavaScript);

        if(minifiedRequestInterceptor.HasErrors)
        {
            throw new UnreachableException("Unable to minify access token request interceptor");
        }

        var encodedRequestInterceptor = HttpUtility.JavaScriptStringEncode(minifiedRequestInterceptor.Code);

        options.UseRequestInterceptor(encodedRequestInterceptor);

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