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
        options.SwaggerDoc("v1", new()
        {
            Title = "Multitenancy Monolith V1 (ticker/api)",
            Version = "v1"
        });

        options.SwaggerDoc("v1-ticker", new()
        {
            Title = "Multitenancy Monolith V1 (ticker/api/ticker)",
            Version = "v1-ticker"
        });

        options.DocInclusionPredicate((docName, apiDesc) =>
        {
            if (docName == "v1")
            {
                return true;
            }

            if (docName.Equals($"v1-{apiDesc.GroupName}"))
            {
                return true;
            }

            return false;
        });

        return options;
    }
}