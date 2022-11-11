using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChristianSchulz.MultitenancyMonolith.Server.BadgeIdentity;

public class SwaggerGenBadgeAuthorizationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var authorizeAttribute = (AuthorizeAttribute?)context.ApiDescription
            .ActionDescriptor
            .EndpointMetadata
            .SingleOrDefault(x => x is AuthorizeAttribute);

        if (authorizeAttribute == null)
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