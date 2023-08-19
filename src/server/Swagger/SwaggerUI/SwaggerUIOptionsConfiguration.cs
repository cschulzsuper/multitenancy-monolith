﻿using ChristianSchulz.MultitenancyMonolith.Configuration;
using Microsoft.AspNetCore.Builder;
using NUglify;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger.SwaggerUI;

internal sealed class SwaggerUIOptionsConfiguration
{
    private const string AccessTokenRequestInterceptorJavaScript =
    """
            function n(req)
            {
                let swaggerJsonRequest = req.url.endsWith('swagger.json');
                if (!swaggerJsonRequest)
                {
                    return req;
                }

                let queryString = new URLSearchParams(window.location.search);
                let accessToken = queryString.get('access_token');
            
                if(accessToken == null)
                {
                    if (document.cookie.length > 0)
                    {
                        let accessTokenName = "access_token";

                        let accessTokenStart = document.cookie.indexOf(accessTokenName + "=");
                        if (accessTokenStart != -1) 
                        {
                            
                            accessTokenStart = accessTokenStart + accessTokenName.length + 1;
                            let accessTokenEnd = document.cookie.indexOf(";", accessTokenStart);
                            
                            if (accessTokenEnd == -1) 
                            {
                                accessTokenEnd = document.cookie.length;
                            }

                            accessToken = unescape(document.cookie.substring(accessTokenStart, accessTokenEnd));
                        }
                    }
                }

                if(accessToken != null)
                {
                    req.headers['Authorization'] = accessToken; 
                }

                return req; 
            }
        """;

    private readonly ISwaggerDocsProvider _swaggerDocsProvider;
    private readonly IServiceMappingsProvider _serviceMappingsProvider;
    private readonly SwaggerJsonClientFactory _swaggerJsonClientFactory;

    public SwaggerUIOptionsConfiguration(
        ISwaggerDocsProvider swaggerDocsProvider,
        IServiceMappingsProvider serviceMappingsProvider,
        SwaggerJsonClientFactory swaggerJsonClientFactory) 
    {
        _swaggerDocsProvider = swaggerDocsProvider;
        _serviceMappingsProvider = serviceMappingsProvider;
        _swaggerJsonClientFactory = swaggerJsonClientFactory;
    }

    public void Configure(SwaggerUIOptions options)
    {
        ConfigureSwaggerEndpoints(options);
        ConfigureAccessTokenRequestInterceptor(options);
    }

    private void ConfigureSwaggerEndpoints(SwaggerUIOptions options)
    {
        var swaggerDocGroups = _swaggerDocsProvider.Get()
            .GroupBy(swaggerDoc => swaggerDoc.TestService);

        foreach (var swaggerDocGroup in swaggerDocGroups)
        {
            using var swaggerJsonClient = _swaggerJsonClientFactory.Create(swaggerDocGroup.Key);

            foreach (var swaggerDoc in swaggerDocGroup)
            {
                var success = swaggerJsonClient.Test(swaggerDoc.Path);
                if (success)
                {
                    var @public = _serviceMappingsProvider.Get().Single(x => x.UniqueName == swaggerDoc.PublicService).Url;

                    var rootUrl = new Uri(@public);
                    var absUrl = new Uri(rootUrl, swaggerDoc.Path);

                    options.SwaggerEndpoint(absUrl.AbsoluteUri, swaggerDoc.DisplayName);
                }
            }
        }
    }

    private static void ConfigureAccessTokenRequestInterceptor(SwaggerUIOptions options)
    {
        var minifiedRequestInterceptor =  Uglify.Js(AccessTokenRequestInterceptorJavaScript);

        if (minifiedRequestInterceptor.HasErrors)
        {
            throw new UnreachableException("Unable to minify access token request interceptor");
        }

        var encodedRequestInterceptor = HttpUtility.JavaScriptStringEncode(minifiedRequestInterceptor.Code);

        options.UseRequestInterceptor(encodedRequestInterceptor);
    }
}
