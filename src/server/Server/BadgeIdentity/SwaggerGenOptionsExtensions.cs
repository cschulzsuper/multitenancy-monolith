using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChristianSchulz.MultitenancyMonolith.Server.BadgeIdentity;

public static class SwaggerGenOptionsExtensions
{

    public static SwaggerGenOptions ConfigureBadgeAuthentication(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Scheme = "Bearer",
            Type = SecuritySchemeType.Http,
            In = ParameterLocation.Header
        });

        return options;
    }

    public static SwaggerGenOptions ConfigureBadgeAuthorization(this SwaggerGenOptions options)
    {
        options.OperationFilter<SwaggerGenBadgeAuthorizationOperationFilter>();

        return options;
    }
}