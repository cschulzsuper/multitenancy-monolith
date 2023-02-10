using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Application.Authorization;
using ChristianSchulz.MultitenancyMonolith.Application.Business;
using ChristianSchulz.MultitenancyMonolith.Caching;
using ChristianSchulz.MultitenancyMonolith.Server.Security.Authentication.Badge;
using ChristianSchulz.MultitenancyMonolith.Server.SwaggerGen;
using ChristianSchulz.MultitenancyMonolith.Server.SwaggerUI;
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
using ChristianSchulz.MultitenancyMonolith.Server.JsonConversion;

namespace ChristianSchulz.MultitenancyMonolith.Server;

public class Startup
{
    private readonly IWebHostEnvironment _environment;

    public Startup(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => { options.SerializerOptions.Converters.Add(new ObjectJsonConverter()); });

        services.AddAuthentication().AddBadge(options => options.Configure());
        services.AddRequestUser();
        services.AddAuthorization();

        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();

        services.AddSwaggerGen(options =>
        {
            options.ConfigureSwaggerDocs();
            options.ConfigureAuthentication();
            options.ConfigureAuthorization();
        });

        services.AddCaching();
        services.AddData();

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
            app.ApplicationServices.ConfigureData();
        }

        app.UseExceptionHandler(appBuilder => appBuilder.Run(HandleError));

        app.UseHttpsRedirection();

        if (!_environment.IsProduction())
        {
            app.UseSwaggerUI(options =>
            {
                options.ConfigureSwaggerEndpoints();
                options.UseAccessTokenRequestInterceptor();
            });
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            if (_environment.IsDevelopment())
            {
                endpoints.MapSwagger();
            }
            else
            {
                endpoints.MapSwagger()
                    .RequireAuthorization(ploicy => ploicy
                        .RequireClaim("scope", "swagger-json"));
            }

            var apiEndpoints = endpoints.MapGroup("api");

            apiEndpoints.MapAdministrationEndpoints();
            apiEndpoints.MapAuthorizationEndpoints();
            apiEndpoints.MapAuthenticationEndpoints();
            apiEndpoints.MapBusinessEndpoints();
        });
    }

    public async Task HandleError(HttpContext context)
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