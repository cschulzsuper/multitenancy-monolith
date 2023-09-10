using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Frontend.Swagger.Endpoints;
using ChristianSchulz.MultitenancyMonolith.Frontend.Swagger.Security;
using ChristianSchulz.MultitenancyMonolith.Frontend.Swagger.SwaggerUI;
using ChristianSchulz.MultitenancyMonolith.Web;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Swagger;

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

        if (!environment.IsDevelopment())
        {
            // TODO https://github.com/dotnet/aspnetcore/issues/49577
            StaticWebAssetsLoader.UseStaticWebAssets(environment, configuration);
        }
    }

    private void LoadRequiredConfiguration(
        out string[] allowedClientHosts,
        out string admissionClientName,
        out string admissionFrontendUrl,
        out string[] webServices)
    {
        var configurationProxyProvider = new ConfigurationProxyProvider(_configuration);

        var configuredAdmissionServer = configurationProxyProvider.GetAdmissionServer();
        var configuredAdmissionPortal = configurationProxyProvider.GetAdmissionPortal();
        var configuredSwaggerDocs = configurationProxyProvider.GetSwaggerDocs();
        var configuredServicesMappings = configurationProxyProvider.GetServiceMappings();
        var configuredAllowedClients = configurationProxyProvider.GetAllowedClients();

        webServices = configuredServicesMappings
            .Where(servicesMapping =>
                servicesMapping.UniqueName == configuredAdmissionServer.Service ||
                configuredSwaggerDocs.Select(swaggerDoc => swaggerDoc.TestService).Contains(servicesMapping.UniqueName))
            .Select(x => x.UniqueName)
            .Distinct()
            .ToArray();

        allowedClientHosts = configuredServicesMappings
            .Where(serviceMapping => configuredAllowedClients
                .Select(allowedClient => allowedClient.Service)
                .Contains(serviceMapping.UniqueName))
            .Select(servicesMapping => servicesMapping.Url)
            .ToArray();

        admissionClientName = configuredAdmissionPortal.ClientName;

        admissionFrontendUrl = configuredServicesMappings
            .Where(servicesMapping =>
                servicesMapping.UniqueName == configuredAdmissionPortal.Service)
            .Select(x => x.Url)
            .Single();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        LoadRequiredConfiguration(
            out var allowedClientHosts,
            out var admissionClientName,
            out var admissionFrontendUrl,
            out var webServices);

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddDataProtection().SetApplicationName(nameof(MultitenancyMonolith));
        services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
            .AddBearerToken(options =>
                {
                    options.Configure();
                    options.ForwardChallenge = CookieAuthenticationDefaults.AuthenticationScheme;
                })
            .AddCookie(options =>
                {
                    options.LoginPath = "/auth";
                    options.ReturnUrlParameter = "return";
                    options.Cookie.Name = "access_token";
                    options.Events = new CookieAuthenticationEvents()
                    {
                        OnRedirectToLogin = (context) =>
                        {
                            var _originalScheme = context.Request.Scheme;
                            var _originalHost = context.Request.Host;
                            var _originalPath = context.Request.Path;
                            var _originalPathBase = context.Request.PathBase;

                            var redirectUri = $"{_originalScheme}://{_originalHost}{_originalPathBase}{_originalPath}{context.Request.QueryString}";

                            var queryParameter = new[]
                            {
                                new KeyValuePair<string, string?>(options.ReturnUrlParameter, redirectUri),
                                new KeyValuePair<string, string?>("client-name", admissionClientName)
                            };

                            var loginUri = $"{admissionFrontendUrl}{options.LoginPath}{QueryString.Create(queryParameter)}";

                            context.HttpContext.Response.Redirect(loginUri);

                            return Task.CompletedTask;
                        }
                    };
                });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = _environment.IsDevelopment()
                ? options.FallbackPolicy
                : options.DefaultPolicy;
        });

        services.AddCors(setup => setup
            .AddDefaultPolicy(config => config
                .WithOrigins(allowedClientHosts)
                .WithMethods(HttpMethods.Get)));

        services.AddScoped<SwaggerUIOptionsConfiguration>();

        services.AddConfiguration();

        services.AddWebServices(webServices);
        services.AddSwaggerJsonWebServiceClients();

        services.AddTransportWebServiceClientFactory();
        services.AddAdmissionTransportWebServiceClients();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseForwardedHeaders();

        app.UseCors();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwaggerUI(options =>
        {
            using var scope = app.ApplicationServices.CreateScope();

            scope.ServiceProvider
                .GetRequiredService<SwaggerUIOptionsConfiguration>()
                .Configure(options);
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapSignIn();
        });
    }
}
