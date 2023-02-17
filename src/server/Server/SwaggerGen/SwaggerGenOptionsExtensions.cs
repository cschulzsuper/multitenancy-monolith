using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChristianSchulz.MultitenancyMonolith.Server.SwaggerGen;

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
        options.SwaggerDoc("v1-server", new()
        {
            Title = "Multitenancy Monolith V1 (server/api)",
            Version = "v1-server"
        });

        options.SwaggerDoc("v1-server-administration", new()
        {
            Title = "Multitenancy Monolith V1 (server/api/administration)",
            Version = "v1-server-administration"
        });

        options.SwaggerDoc("v1-server-authentication", new()
        {
            Title = "Multitenancy Monolith V1 (server/api/authentication)",
            Version = "v1-server-authentication"
        });

        options.SwaggerDoc("v1-server-authorization", new()
        {
            Title = "Multitenancy Monolith V1 (server/api/authorization)",
            Version = "v1-server-authorization"
        });

        options.SwaggerDoc("v1-server-business", new()
        {
            Title = "Multitenancy Monolith V1 (server/api/business)",
            Version = "v1-server-business"
        });

        options.DocInclusionPredicate((docName, apiDesc) =>
        {
            if (docName == "v1-server")
            {
                return true;
            }

            if (docName.Equals($"v1-server-{apiDesc.GroupName}"))
            {
                return true;
            }

            return false;
        });

        return options;
    }
}