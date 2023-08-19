using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Server.Swagger.Blazor;
using ChristianSchulz.MultitenancyMonolith.Server.Swagger.Endpoints;
using ChristianSchulz.MultitenancyMonolith.Server.Swagger.Security;
using ChristianSchulz.MultitenancyMonolith.Server.Swagger.SwaggerUI;
using ChristianSchulz.MultitenancyMonolith.Web;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger;

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
        out string[] webServices)
    {
        var configurationProxyProvider = new ConfigurationProxyProvider(_configuration);

        var configuredAdmissionServer = configurationProxyProvider.GetAdmissionServer();
        var configuredSwaggerDocs = configurationProxyProvider.GetSwaggerDocs();
        var configuredServicesMappings = configurationProxyProvider.GetServiceMappings();

        webServices = configuredServicesMappings
            .Where(servicesMapping =>
                servicesMapping.UniqueName == configuredAdmissionServer.Service ||
                configuredSwaggerDocs.Select(swaggerDoc => swaggerDoc.TestService).Contains(servicesMapping.UniqueName))
            .Select(x => x.UniqueName)
            .Distinct()
            .ToArray();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        LoadRequiredConfiguration(
            out var webServices);

        if (!_environment.IsDevelopment())
        {
            services.AddHttpsRedirection(options =>
            {
                var httpsRedirectPortSetting = Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_REDIRECT_PORT");
                var httpsRedirectPortValid = ushort.TryParse(httpsRedirectPortSetting, out ushort httpsRedirectPort);
                if (httpsRedirectPortValid) 
                {
                    options.HttpsPort = httpsRedirectPort;
                }
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
                        options.LoginPath = "/sign-in";
                        options.ReturnUrlParameter = "return";
                        options.Cookie.Name = "access_token";
                    });

            services.AddAuthorization(options => options.FallbackPolicy = options.DefaultPolicy);
        }

        services.AddRazorComponents();
        services.AddCors();

        services.AddScoped<SwaggerUIOptionsConfiguration>();

        services.AddConfiguration();

        services.AddWebServices(webServices);
        services.AddWebServiceSwaggerJsonClients();
        services.AddWebServiceTransportClients();

        // TODO https://github.com/dotnet/aspnetcore/issues/48769
        services.AddHttpContextAccessor();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseHttpsRedirection();

        app.UseCors();
        app.UseStaticFiles();

        if (!_environment.IsDevelopment())
        {
            app.UseRouting();
            app.UseAntiforgery();

            app.UseAuthentication();
            app.UseAuthorization();
        }
      
        app.UseSwaggerUI(options =>
        {         
            using var scope = app.ApplicationServices.CreateScope();

            scope.ServiceProvider
                .GetRequiredService<SwaggerUIOptionsConfiguration>()
                .Configure(options);           
        });

        if (!_environment.IsDevelopment())
        {
            app.UseEndpoints(endpoints =>
            {
                var group = endpoints
                    .MapGroup("/")
                    .AllowAnonymous();

                group.MapRazorComponents<App>();
                group.MapIndex();
            });
        }
    }
}
