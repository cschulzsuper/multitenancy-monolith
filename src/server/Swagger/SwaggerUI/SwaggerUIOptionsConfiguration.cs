using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Web;
using Microsoft.AspNetCore.Builder;
using NUglify;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger.SwaggerUI;

public sealed class SwaggerUIOptionsConfiguration
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

    private readonly ISwaggerDocsProvider _swaggerDocsProvider;
    private readonly IServiceMappingsProvider _serviceMappingsProvider;
    private readonly IWebServiceClientFactory _webServiceClientFactory;

    public SwaggerUIOptionsConfiguration(
        ISwaggerDocsProvider swaggerDocsProvider,
        IServiceMappingsProvider serviceMappingsProvider,
        IWebServiceClientFactory webServiceClientFactory) 
    {
        _swaggerDocsProvider = swaggerDocsProvider;
        _serviceMappingsProvider = serviceMappingsProvider;
        _webServiceClientFactory = webServiceClientFactory;
    }

    public void Configure(SwaggerUIOptions options)
    {
        ConfigureSwaggerEndpoints(options);
        ConfigureAccessTokenRequestInterceptor(options);
    }

    public void ConfigureSwaggerEndpoints(SwaggerUIOptions options)
    {
        var swaggerDocs = _swaggerDocsProvider.Get();

        foreach (var swaggerDoc in swaggerDocs)
        {
            using var webServiceClient = _webServiceClientFactory.Create(swaggerDoc.Service);

            var webServiceStatusCodeResult = webServiceClient.TryGet(swaggerDoc.Path);
            if (webServiceStatusCodeResult.IsSuccessStatusCode)
            {
                var @public = _serviceMappingsProvider.Get().Single(x => x.UniqueName == swaggerDoc.Service).PublicUrl;

                var rootUrl = new Uri(@public);
                var absUrl = new Uri(rootUrl,swaggerDoc.Path);

                options.SwaggerEndpoint(absUrl.AbsoluteUri, swaggerDoc.DisplayName);
            }
        }
    }

    public void ConfigureAccessTokenRequestInterceptor(SwaggerUIOptions options)
    {
        var minifiedRequestInterceptor = Uglify.Js(AccessTokenRequestInterceptorJavaScript);

        if (minifiedRequestInterceptor.HasErrors)
        {
            throw new UnreachableException("Unable to minify access token request interceptor");
        }

        var encodedRequestInterceptor = HttpUtility.JavaScriptStringEncode(minifiedRequestInterceptor.Code);

        options.UseRequestInterceptor(encodedRequestInterceptor);
    }
}
