using ChristianSchulz.MultitenancyMonolith.Backend.Server.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Backend.Server.SwaggerGen;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenApiDocs(this IServiceCollection services)
    {
        services.AddKeyedSingleton<OpenApiSchemaProvider>("a1");
        services.AddOpenApi("a1", options =>
        {
            options.ShouldInclude = apiDesc => true;
        });

        services.AddKeyedSingleton<OpenApiSchemaProvider>("a1-access");
        services.AddOpenApi("a1-access", options => 
        {
            options.ShouldInclude = apiDesc => apiDesc.GroupName == "access";
            options.AddDocumentTitle("Server A1");
            options.AddDocumentAuthorization();
            options.AddOperationStatusCodes();
            options.AddOperationAuthorization();
        });

        services.AddKeyedSingleton<OpenApiSchemaProvider>("a1-admission");
        services.AddOpenApi("a1-admission", options =>
        {
            options.ShouldInclude = apiDesc => apiDesc.GroupName == "admission";
            options.AddDocumentTitle("Server A1 (admission)");
            options.AddDocumentAuthorization();
            options.AddOperationStatusCodes();
            options.AddOperationAuthorization();
        });

        services.AddKeyedSingleton<OpenApiSchemaProvider>("a1-business");
        services.AddOpenApi("a1-business", options =>
        {
            options.ShouldInclude = apiDesc => apiDesc.GroupName == "business";
            options.AddDocumentTitle("Server A1 (business)");
            options.AddDocumentAuthorization();
            options.AddOperationStatusCodes();
            options.AddOperationAuthorization();
        });

        services.AddKeyedSingleton<OpenApiSchemaProvider>("a1-extension");
        services.AddOpenApi("a1-extension", options =>
        {
            options.ShouldInclude = apiDesc => apiDesc.GroupName == "extension";
            options.AddDocumentTitle("Server A1 (extension)");
            options.AddDocumentAuthorization();
            options.AddOperationStatusCodes();
            options.AddOperationAuthorization();
        });

        services.AddKeyedSingleton<OpenApiSchemaProvider>("a1-diagnostic");
        services.AddOpenApi("a1-diagnostic", options =>
        {
            options.ShouldInclude = apiDesc => apiDesc.GroupName == "diagnostic";
            options.AddDocumentTitle("Server A1 (diagnostic)");
            options.AddDocumentAuthorization();
            options.AddOperationStatusCodes();
            options.AddOperationAuthorization();
        });

        services.AddKeyedSingleton<OpenApiSchemaProvider>("a1-schedule");
        services.AddOpenApi("a1-schedule", options =>
        {
            options.ShouldInclude = apiDesc => apiDesc.GroupName == "schedule";
            options.AddDocumentTitle("Server A1 (schedule)");
            options.AddDocumentAuthorization();
            options.AddOperationStatusCodes();
            options.AddOperationAuthorization();
        });

        return services;
    }
}