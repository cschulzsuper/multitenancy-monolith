using System;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ChristianSchulz.MultitenancyMonolith.Server.SwaggerUI;

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

    public static SwaggerUIOptions ConfigureSwaggerEndpoints(this SwaggerUIOptions options)
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json",
            "Multitenancy Monolith V1 (api)");

        options.SwaggerEndpoint("/swagger/v1-administration/swagger.json",
            "Multitenancy Monolith V1 (api/administration)");

        options.SwaggerEndpoint("/swagger/v1-authentication/swagger.json",
            "Multitenancy Monolith V1 (api/authentication)");

        options.SwaggerEndpoint("/swagger/v1-authorization/swagger.json",
            "Multitenancy Monolith V1 (api/authorization)");

        options.SwaggerEndpoint("/swagger/v1-business/swagger.json",
            "Multitenancy Monolith V1 (api/business)");

        return options;
    }
}