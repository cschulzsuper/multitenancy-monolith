using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Application.Schedule;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker;
using ChristianSchulz.MultitenancyMonolith.Caching;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Jobs;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Events;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Jobs;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Json;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Middleware;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.SwaggerGen;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;
using ChristianSchulz.MultitenancyMonolith.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.Ticker;

public sealed class Startup
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    private readonly ICollection<AllowedClient> _allowedClients;
    private readonly string[] _allowedClientHosts;

    private readonly string[] _services;

    public Startup(
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;

        _services = new ServiceMappingsProvider(configuration)
            .GetUniqueNames()
            .Where(x => x == new AuthenticationServerProvider(configuration).Get().Service)
            .ToArray();

        _allowedClients = new AllowedClientsProvider(_configuration).Get();

        _allowedClientHosts = new ServiceMappingsProvider(_configuration)
            .Get()
            .Where(serviceMapping => _allowedClients
                .Select(allowedClient => allowedClient.Service)
                .Contains(serviceMapping.UniqueName))
            .Select(x => x.PublicUrl)
            .ToArray();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureJsonOptions();

        services.AddAuthentication().AddBearerToken(options => options.Configure());
        services.AddAuthorization();

        services.AddCors();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.ConfigureSwaggerDocs();
            options.ConfigureAuthentication();
            options.ConfigureAuthorization();
        });

        services.AddWebServices(_services);

        services.AddRequestUser(options => options.Configure(_allowedClients));
        services.AddCaching();
        services.AddConfiguration();
        services.AddEvents(options => options.Configure());
        services.AddPlannedJobs(options => options.Configure());

        services.AddStaticDictionary();
        services.AddStaticDictionaryScheduleData();
        services.AddStaticDictionaryTickerData();

        services.AddScheduleManagement();
        services.AddScheduleTransport();
        services.AddScheduleOrchestration();

        services.AddTickerManagement();
        services.AddTickerTransport();
        services.AddTickerOrchestration();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.ApplicationServices
            .GetRequiredService<IEventSubscriptions>()
            .MapScheduleSubscriptions()
            .MapTickerSubscriptions();

        app.ConfigureJobScheduler()
            .MapHeartbeat();

        if (!_environment.IsProduction())
        {
            app.ApplicationServices.ConfigureTickerUsers();
        }

        app.UseExceptionHandler(appBuilder => appBuilder.Run(HandleError));

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors(config => config
            .WithOrigins(_allowedClientHosts)
            .WithHeaders(HeaderNames.Accept, HeaderNames.ContentType, HeaderNames.Authorization)
            .WithMethods(HttpMethods.Get, HttpMethods.Head, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete));

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

            apiEndpoints.MapTickerEndpoints();
            apiEndpoints.MapScheduleEndpoints();
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