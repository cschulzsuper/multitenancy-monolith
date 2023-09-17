using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChristianSchulz.MultitenancyMonolith.Server.SwaggerGen
{
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
            options.SwaggerDoc("a1", new()
            {
                Title = "Server A1",
                Version = "a1"
            });

            options.SwaggerDoc("a1-access", new()
            {
                Title = "Server A1 (access)",
                Version = "a1-access"
            });

            options.SwaggerDoc("a1-admission", new()
            {
                Title = "Server A1 (admission)",
                Version = "a1-admission"
            });

            options.SwaggerDoc("a1-business", new()
            {
                Title = "Server A1 (business)",
                Version = "a1-business"
            });

            options.SwaggerDoc("a1-extension", new()
            {
                Title = "Server A1 (extension)",
                Version = "a1-extension"
            });

            options.SwaggerDoc("a1-schedule", new()
            {
                Title = "Server A1 (schedule)",
                Version = "a1-schedule"
            });

            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (docName == "a1")
                {
                    return true;
                }

                if (docName.Equals($"a1-{apiDesc.GroupName}"))
                {
                    return true;
                }

                return false;
            });

            return options;
        }
    }
}