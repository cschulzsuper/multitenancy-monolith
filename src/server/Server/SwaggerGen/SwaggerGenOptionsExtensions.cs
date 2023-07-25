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
        options.SwaggerDoc("v1", new()
        {
            Title = "Multitenancy Monolith V1",
            Version = "v1"
        });

        options.SwaggerDoc("v1-access", new()
        {
            Title = "Multitenancy Monolith V1 (access)",
            Version = "v1-access"
        });

        options.SwaggerDoc("v1-admission", new()
        {
            Title = "Multitenancy Monolith V1 (admission)",
            Version = "v1-admission"
        });

        options.SwaggerDoc("v1-business", new()
        {
            Title = "Multitenancy Monolith V1 (business)",
            Version = "v1-business"
        });

        options.SwaggerDoc("v1-extension", new()
        {
            Title = "Multitenancy Monolith V1 (extension)",
            Version = "v1-extension"
        });

        options.SwaggerDoc("v1-schedule", new()
        {
            Title = "Multitenancy Monolith V1 (schedule)",
            Version = "v1-schedule"
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