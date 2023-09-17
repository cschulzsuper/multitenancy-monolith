using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Application.Access;
using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Application.Business;
using ChristianSchulz.MultitenancyMonolith.Application.Extension;
using ChristianSchulz.MultitenancyMonolith.Application.Schedule;
using ChristianSchulz.MultitenancyMonolith.Caching;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Jobs;
using ChristianSchulz.MultitenancyMonolith.Server.DataProtection;
using ChristianSchulz.MultitenancyMonolith.Server.Events;
using ChristianSchulz.MultitenancyMonolith.Server.Jobs;
using ChristianSchulz.MultitenancyMonolith.Server.Json;
using ChristianSchulz.MultitenancyMonolith.Server.Middleware;
using ChristianSchulz.MultitenancyMonolith.Server.Security;
using ChristianSchulz.MultitenancyMonolith.Server.SwaggerGen;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;
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
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server
{
    public sealed class Startup
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

        private void LoadRequiredConfiguration(
            out AllowedClient[] allowedClients,
            out string[] allowedClientHosts)
        {
            var configurationProxyProvider = new ConfigurationProxyProvider(_configuration);

            var configuredAllowedClients = configurationProxyProvider.GetAllowedClients();
            var configuredServicesMappings = configurationProxyProvider.GetServiceMappings();

            allowedClients = configuredAllowedClients;

            allowedClientHosts = configuredServicesMappings
                .Where(serviceMapping => configuredAllowedClients
                    .Select(allowedClient => allowedClient.Service)
                    .Contains(serviceMapping.UniqueName))
                .Select(servicesMapping => servicesMapping.Url)
                .ToArray();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            LoadRequiredConfiguration(
                out var allowedClients,
                out var allowedClientHosts);

            services.ConfigureJsonOptions();

            services.AddDataProtection().Configure(_environment, _configuration);

            services.AddAuthentication().AddBearerToken(options => options.Configure());
            services.AddAuthorization();

            services.AddCors(setup => setup
                .AddDefaultPolicy(config => config
                    .WithOrigins(allowedClientHosts)
                    .WithHeaders(HeaderNames.Accept, HeaderNames.ContentType, HeaderNames.Authorization)
                    .WithMethods(HttpMethods.Get, HttpMethods.Head, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete)));

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.ConfigureSwaggerDocs();
                options.ConfigureAuthentication();
                options.ConfigureAuthorization();
            });

            services.AddRequestUser(options => options.Configure(allowedClients));
            services.AddCaching();
            services.AddConfiguration();
            services.AddEvents(options => options.Configure());
            services.AddPlannedJobs(options => options.Configure());

            services.AddStaticDictionary();
            services.AddStaticDictionaryAccessData();
            services.AddStaticDictionaryAdmissionData();
            services.AddStaticDictionaryExtensionData();
            services.AddStaticDictionaryBusinessData();
            services.AddStaticDictionaryScheduleData();

            services.AddAccessManagement();
            services.AddAccessTransport();

            services.AddAdmissionManagement();
            services.AddAdmissionTransport();

            services.AddExtensionManagement();
            services.AddExtensionTransport();

            services.AddBusinessManagement();
            services.AddBusinessTransport();

            services.AddScheduleManagement();
            services.AddScheduleTransport();
            services.AddScheduleOrchestration();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.ApplicationServices
                .GetRequiredService<IEventSubscriptions>()
                .MapScheduleSubscriptions();

            app.ConfigureJobScheduler()
                .MapHeartbeat();

            if (!_environment.IsProduction())
            {
                app.ApplicationServices.ConfigureAuthenticationIdentities();
                app.ApplicationServices.ConfigureAuthenticationIdentityAuthenticationMethods();
                app.ApplicationServices.ConfigureAccountGroups();
                app.ApplicationServices.ConfigureAccountMembers();
            }

            app.UseExceptionHandler(appBuilder => appBuilder.Run(HandleError));

            app.UseCors();
            app.UseRouting();

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

                var apiEndpoints = endpoints.MapGroup("api/a1");

                apiEndpoints.MapAdmissionEndpoints();
                apiEndpoints.MapAccessEndpoints();
                apiEndpoints.MapBusinessEndpoints();
                apiEndpoints.MapExtensionEndpoints();
                apiEndpoints.MapScheduleEndpoints();
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

            await context.Response.WriteAsJsonAsync(problem, (JsonSerializerOptions?)null, "application/problem+json");
        }
    }
}