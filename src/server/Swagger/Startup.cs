using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Server.Swagger.Endpoints;
using ChristianSchulz.MultitenancyMonolith.Server.Swagger.Security;
using ChristianSchulz.MultitenancyMonolith.Server.Swagger.SwaggerUI;
using ChristianSchulz.MultitenancyMonolith.Web;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger;

public sealed class Startup
{
    private readonly IWebHostEnvironment _environment;
    private readonly string[] _services;

    public Startup(
        IWebHostEnvironment environment, 
        IConfiguration configuration)
    {
        _environment = environment;

        _services = new ServiceMappingsProvider(configuration)
            .GetUniqueNames()
            .Where(service => 
                new SwaggerDocsProvider(configuration)
                    .Get()
                    .Select(swaggerDoc => swaggerDoc.TestService)
                    .Contains(service) ||
                service == new AdmissionServerProvider(configuration).Get().Service)
            .Distinct()
            .ToArray();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddConfiguration();

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

        services.AddScoped<SwaggerUIOptionsConfiguration>();

        services.AddWebServices(_services);
        services.AddWebServiceSwaggerJsonClients();
        services.AddWebServiceTransportClients();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseHttpsRedirection();

        if (!_environment.IsDevelopment())
        {
            app.UseRouting();

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
                endpoints.MapIndex();
                endpoints.MapSignIn();
            });
        }
    }
}