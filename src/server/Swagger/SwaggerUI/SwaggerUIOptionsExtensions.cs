using System;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;

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

    public static SwaggerUIOptions ConfigureSwaggerEndpoints(this SwaggerUIOptions options)
    {
        options.SwaggerEndpoint("https://localhost:7207/swagger/v1-server/swagger.json",
            "Multitenancy Monolith V1 (server/api)");

        options.SwaggerEndpoint("https://localhost:7207/swagger/v1-server-administration/swagger.json",
            "Multitenancy Monolith V1 (server/api/administration)");

        options.SwaggerEndpoint("https://localhost:7207/swagger/v1-server-authentication/swagger.json",
            "Multitenancy Monolith V1 (server/api/authentication)");

        options.SwaggerEndpoint("https://localhost:7207/swagger/v1-server-authorization/swagger.json",
            "Multitenancy Monolith V1 (server/api/authorization)");

        options.SwaggerEndpoint("https://localhost:7207/swagger/v1-server-business/swagger.json",
            "Multitenancy Monolith V1 (server/api/business)");

        options.SwaggerEndpoint("https://localhost:7206/swagger/v1-ticker/swagger.json",
            "Multitenancy Monolith V1 (ticker/api)");

        options.SwaggerEndpoint("https://localhost:7206/swagger/v1-ticker-ticker/swagger.json",
            "Multitenancy Monolith V1 (ticker/api/ticker)");

        return options;
    }
}