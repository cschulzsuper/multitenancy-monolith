using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChristianSchulz.MultitenancyMonolith.Server.Ticker.SwaggerGen;

internal static class SwaggerGenOptionsExtensions
{

    public static SwaggerGenOptions ConfigureAuthentication(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Scheme = "Bearer",
            Type = SecuritySchemeType.Http,
            In = ParameterLocation.Header
        });

        return options;
    }

    public static SwaggerGenOptions ConfigureAuthorization(this SwaggerGenOptions options)
    {
        options.OperationFilter<StatusCodeOperationFilter>();
        options.OperationFilter<AuthorizationOperationFilter>();

        return options;
    }

    public static SwaggerGenOptions ConfigureSwaggerDocs(this SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1-ticker", new()
        {
            Title = "Multitenancy Monolith V1 (ticker/api)",
            Version = "v1-ticker"
        });

        options.SwaggerDoc("v1-ticker-ticker", new()
        {
            Title = "Multitenancy Monolith V1 (ticker/api/ticker)",
            Version = "v1-ticker-ticker"
        });

        options.DocInclusionPredicate((docName, apiDesc) =>
        {
            if (docName == "v1-ticker")
            {
                return true;
            }

            if (docName.Equals($"v1-ticker-{apiDesc.GroupName}"))
            {
                return true;
            }

            return false;
        });

        return options;
    }
}