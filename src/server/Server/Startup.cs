using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Application.Weather;
using ChristianSchulz.MultitenancyMonolith.Caching;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Server.Security.Authentication.Badge;
using ChristianSchulz.MultitenancyMonolith.Server.SwaggerGen;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;
using ChristianSchulz.MultitenancyMonolith.Application;

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
        services.AddAuthentication().AddBadge(options => options.Configure());
        services.AddRequestUser();
        services.AddAuthorization();

        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "Multitenancy Monolith", Version = "v1" });

            options.ConfigureAuthentication();
            options.ConfigureAuthorization();
        });

        services.AddCaching();
        services.AddData();

        services.AddAdministrationManagement();
        services.AddAdministrationTransport();

        services.AddAuthenticationManagement();
        services.AddAuthenticationTransport();

        services.AddWeatherForecastTransport();
    }

    public void Configure(IApplicationBuilder app)
    {
        if (_environment.IsDevelopment())
        {
            app.ApplicationServices.ConfigureData();
        }

        app.UseExceptionHandler(appBuilder => appBuilder.Run(HandleError));

        app.UseHttpsRedirection();

        if (_environment.IsDevelopment())
        {
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Multitenancy Monolith");
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

            endpoints.MapAdministrationEndpoints();
            endpoints.MapAuthenticationEndpoints();
            endpoints.MapWeatherForecastEndpoints();
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

            var httpMethod = context.Request.Method;
            var statusCode = httpMethod switch
            {
                _ when HttpMethods.IsGet(httpMethod) => StatusCodes.Status404NotFound,
                _ when HttpMethods.IsHead(httpMethod) => StatusCodes.Status404NotFound,
                _ when HttpMethods.IsPost(httpMethod) => StatusCodes.Status400BadRequest,
                _ when HttpMethods.IsPut(httpMethod) => StatusCodes.Status400BadRequest,
                _ when HttpMethods.IsPatch(httpMethod) => StatusCodes.Status400BadRequest,
                _ when HttpMethods.IsDelete(httpMethod) => StatusCodes.Status400BadRequest,
                _ => throw exception
            };

            var errorMessageAttribute = context.GetEndpoint()?.Metadata
                .GetMetadata<ErrorMessageAttribute>();

            context.Response.StatusCode = statusCode;

            problem = new ProblemDetails
            {
                Instance = context.Request.Path,
                Title = errorMessageAttribute?.ErrorMessage ?? "Could not process request",
                Status = statusCode,
                Detail = exception.Message
            };
        }

        await context.Response.WriteAsJsonAsync(problem, (JsonSerializerOptions?)null, "application/problem+json");
    }
}