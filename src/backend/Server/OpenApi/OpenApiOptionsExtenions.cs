using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Backend.Server.SwaggerGen;

internal static class OpenApiOptionsExtenions
{
    public static OpenApiOptions AddDocumentTitle(this OpenApiOptions options, string title)
    {
        options.UseTransformer((document, context, cancellationToken) =>
        {
            document.Info.Title = title;

            return Task.CompletedTask;
        });

        return options;
    }

    public static OpenApiOptions AddDocumentAuthorization(this OpenApiOptions options)
    {
        options.UseTransformer((document, context, cancellationToken) =>
        {
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header
                }
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;

            return Task.CompletedTask;
        });

        return options;
    }

    public static OpenApiOptions AddOperationStatusCodes(this OpenApiOptions options)
    {
        options.UseOperationTransformer((operation, context, cancellationToken) =>
        {
            var componentServiceType = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(assembly => assembly.GetType("Microsoft.AspNetCore.OpenApi.OpenApiComponentService", false))
                .SingleOrDefault(type => type != null)!;

            var componentService = context.ApplicationServices.GetRequiredKeyedService(componentServiceType, context.DocumentName);

            var getOrCreateSchema = componentServiceType.GetMethod("GetOrCreateSchema", BindingFlags.NonPublic | BindingFlags.Instance)!;

            var schema = getOrCreateSchema.Invoke(componentService, [typeof(ProblemDetails), (ApiParameterDescription?)null]) as OpenApiSchema;

            var errorStatusCodeContent = new Dictionary<string, OpenApiMediaType>
            {
                ["application/problem+json"] = new OpenApiMediaType
                {
                    Schema = schema
                }
            };

            var method = context.Description.HttpMethod ?? string.Empty;

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

            var hasAuthorizeAttribute = context.Description
                .ActionDescriptor
                .EndpointMetadata
                .Any(metadata => metadata is AuthorizeAttribute);

            if (hasAuthorizeAttribute)
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
            }

            return Task.CompletedTask;
        });

        return options;
        
    }

    public static OpenApiOptions AddOperationAuthorization(this OpenApiOptions options)
    {
        options.UseOperationTransformer((operation, context, cancellationToken) =>
        {
            var hasAuthorizeAttribute = context.Description
            .ActionDescriptor
            .EndpointMetadata
            .Any(x => x is AuthorizeAttribute);

            if (!hasAuthorizeAttribute)
            {
                return Task.CompletedTask;
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

            return Task.CompletedTask;
        });

        return options;
    }
}