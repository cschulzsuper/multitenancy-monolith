using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Application.Authorization;
using ChristianSchulz.MultitenancyMonolith.Application.Business;
using ChristianSchulz.MultitenancyMonolith.Caching;
using ChristianSchulz.MultitenancyMonolith.Server.SwaggerGen;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.Server.Security;
using ChristianSchulz.MultitenancyMonolith.Server.Middleware;
using ChristianSchulz.MultitenancyMonolith.Server.Json;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using Microsoft.Extensions.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Server;

public class Startup
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public Startup(
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureJsonOptions();

        services.AddAuthentication().AddBadge(options => options.Configure(new AllowedClientsProvider(_configuration).Get()));
        services.AddAuthorization();

        services.AddCors();

        services.AddRequestUser();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.ConfigureSwaggerDocs();
            options.ConfigureAuthentication();
            options.ConfigureAuthorization();
        });

        services.AddCaching();
        services.AddConfiguration();
        services.AddOrchestration();

        services.AddStaticDictionary();
        services.AddStaticDictionaryAdministrationData();
        services.AddStaticDictionaryAuthenticationData();
        services.AddStaticDictionaryAuthorizationData();
        services.AddStaticDictionaryBusinessData();

        services.AddAdministrationManagement();
        services.AddAdministrationTransport();

        services.AddAuthenticationManagement();
        services.AddAuthenticationTransport();

        services.AddAuthorizationManagement();
        services.AddAuthorizationTransport();

        services.AddBusinessManagement();
        services.AddBusinessTransport();
    }

    public void Configure(IApplicationBuilder app)
    {
        if (!_environment.IsProduction())
        {
            app.ApplicationServices.ConfigureIdentities();
            app.ApplicationServices.ConfigureGroups();
            app.ApplicationServices.ConfigureMembers();
        }

        app.UseExceptionHandler(appBuilder => appBuilder.Run(HandleError));

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors(config => config.WithOrigins("https://localhost:7272"));

        app.UseAuthenticationScope();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpointEvents();
        app.UseEndpoints(endpoints =>
        {
            if (_environment.IsDevelopment())
            {
                endpoints.MapSwagger();
            }
            else
            {
                endpoints.MapSwagger()
                    .RequireAuthorization(policy => policy
                        .RequireClaim("scope", "swagger-json"));
            }

            var apiEndpoints = endpoints.MapGroup("api");

            apiEndpoints.MapAdministrationEndpoints();
            apiEndpoints.MapAuthenticationEndpoints();
            apiEndpoints.MapAuthorizationEndpoints();
            apiEndpoints.MapBusinessEndpoints();
        });
    }

    private static async Task HandleError(HttpContext context)
    {
        ProblemDetails problem;

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature == null)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path
            };
        }
        else
        {
            var exception = exceptionHandlerPathFeature.Error;
            var exceptionErrorCode = exception.Data.Contains("error-code")
                ? exception.Data["error-code"]?.ToString()
                : null;

            var statusCode = (exception, exceptionErrorCode) switch
            {
                (NotImplementedException, _) => StatusCodes.Status501NotImplemented,
                (BadHttpRequestException, _) => StatusCodes.Status400BadRequest,

                (_, "object-not-found") => StatusCodes.Status404NotFound,
                (_, "security") => StatusCodes.Status403Forbidden,
                (_, "object-conflict") => StatusCodes.Status409Conflict,
                (_, "object-invalid") => StatusCodes.Status400BadRequest,
                (_, "value-invalid") => StatusCodes.Status400BadRequest,

                _ => StatusCodes.Status500InternalServerError
            };

            var errorMessageAttribute = context.GetEndpoint()?.Metadata
                .GetMetadata<ErrorMessageAttribute>();

            context.Response.StatusCode = statusCode;

            problem = new ProblemDetails
            {
                Type = exceptionErrorCode,
                Instance = context.Request.Path,
                Title = errorMessageAttribute?.ErrorMessage ?? "Could not process request",
                Status = statusCode,
                Detail = exception.Message
            };
        }

        await context.Response.WriteAsJsonAsync(problem, (JsonSerializerOptions?) null, "application/problem+json");
    }
}