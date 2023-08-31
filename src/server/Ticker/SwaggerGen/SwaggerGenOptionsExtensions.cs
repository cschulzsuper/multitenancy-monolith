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
        options.SwaggerDoc("b1", new()
        {
            Title = "Ticker B1",
            Version = "b1"
        });

        options.SwaggerDoc("b1-schedule", new()
        {
            Title = "Ticker B1 (schedule)",
            Version = "B1-schedule"
        });

        options.SwaggerDoc("b1-ticker", new()
        {
            Title = "Ticker B1 (ticker)",
            Version = "b1-ticker"
        });

        options.DocInclusionPredicate((docName, apiDesc) =>
        {
            if (docName == "b1")
            {
                return true;
            }

            if (docName.Equals($"b1-{apiDesc.GroupName}"))
            {
                return true;
            }

            return false;
        });

        return options;
    }
}