using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Server.SwaggerGen;

internal sealed class AuthorizationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorizeAttribute = context.ApiDescription
            .ActionDescriptor
            .EndpointMetadata
            .Any(x => x is AuthorizeAttribute);

        if (!hasAuthorizeAttribute)
        {
            return;
        }

        var authorizations = new List<string>();

        operation.Security = new List<OpenApiSecurityRequirement>
    {
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                authorizations
            }
        }
    };
    }
}