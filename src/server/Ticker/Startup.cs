using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker;
using ChristianSchulz.MultitenancyMonolith.Caching;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Json;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Middleware;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.SwaggerGen;
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

namespace ChristianSchulz.MultitenancyMonolith.Server.Ticker;

public class Startup
{
    private readonly IWebHostEnvironment _environment;

    public Startup(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureJsonOptions();

        services.AddAuthentication().AddBadge(options => options.Configure());
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

        services.AddStaticDictionary();
        services.AddStaticDictionaryTickerData();

        services.AddTickerManagement();
        services.AddTickerTransport();
    }

    public void Configure(IApplicationBuilder app)
    {
        if (!_environment.IsProduction())
        {
            app.ApplicationServices.ConfigureTickerUsers();
        }

        app.UseExceptionHandler(appBuilder => appBuilder.Run(HandleError));

        app.UseHttpsRedirection();

        if (!_environment.IsProduction())
        {
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.UseCors(config => config.WithOrigins("https://localhost:7272"));

        app.UseAuthenticationScope();
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
                    .RequireAuthorization(policy => policy
                        .RequireClaim("scope", "swagger-json"));
            }

            var apiEndpoints = endpoints.MapGroup("api");

            apiEndpoints.MapTickerEndpoints();
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

                (_, "security") => StatusCodes.Status403Forbidden,
                (_, "object-not-found") => StatusCodes.Status404NotFound,
                (_, "object-conflict") => StatusCodes.Status409Conflict,

                (BadHttpRequestException, _) => StatusCodes.Status400BadRequest,
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

        await context.Response.WriteAsJsonAsync(problem, (JsonSerializerOptions?)null, "application/problem+json");
    }
}