using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Server.SwaggerGen;

internal sealed class StatusCodeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var errorStatusCodeContent = new Dictionary<string, OpenApiMediaType>
        {
            ["application/problem+json"] = new OpenApiMediaType
            {
                Schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository)
            }
        };

        var method = context.ApiDescription.HttpMethod ?? string.Empty;

        if (HttpMethods.IsGet(method) ||
            HttpMethods.IsHead(method))
        {
            operation.Responses.TryAdd("404",
                new OpenApiResponse
                {
                    Description = "Not Found",
                    Content = errorStatusCodeContent
                });
        }

        if (HttpMethods.IsPost(method) ||
            HttpMethods.IsPut(method) ||
            HttpMethods.IsPatch(method) ||
            HttpMethods.IsDelete(method))
        {
            operation.Responses.TryAdd("400",
                new OpenApiResponse
                {
                    Description = "Bad Request",
                    Content = errorStatusCodeContent
                });
        }

        var hasAuthorizeAttribute = context.ApiDescription
            .ActionDescriptor
            .EndpointMetadata
            .Any(x => x is AuthorizeAttribute);

        if (hasAuthorizeAttribute)
        {
            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
        }
    }
}